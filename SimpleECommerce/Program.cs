using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.Models;
using SimpleECommerce.Helpers;
using SimpleECommerce.middlewares;
using SimpleECommerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
//using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

// for using logging 
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.

// configuration to map the data from appsetings to class JWT
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

// configuration to map the data from appsetings to class usedDataForEmailSender
builder.Services.Configure<usedDataForEmailSender>(builder.Configuration.GetSection("usedDataForEmailSender"));

// configuration to map the data from appsetings to class orderStatusData
builder.Services.Configure<orderStatuses>(builder.Configuration.GetSection("orderStatuses"));
// configuration to map the data from appsetings to class AmountOfTimeUserAbleToCancellOrder
builder.Services.Configure<AmountOfTimeUserAbleToCancellOrder>(builder.Configuration.GetSection("AmountOfTimeUserAbleToCancellOrder"));

// assign the connection string variable with it's data
var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
// use this connection string to map between my class db context and my db in the sql server
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// configure our identity(tells our system that which context is for the identity)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// configure services by scoped
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransferPhotosToPathWithStoreService, TransferPhotosToPathWithStoreService>();
builder.Services.AddScoped<IProdService, ProdService>();
builder.Services.AddScoped<ICartOrdersService, CartOrdersService>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

// configure JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(o =>
   {
       o.RequireHttpsMetadata = false;
       o.SaveToken = false;
       o.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuerSigningKey = true,
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = true,
           ValidIssuer = builder.Configuration["JWT:Issuer"],
           ValidAudience = builder.Configuration["JWT:Audience"],
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
       };
   });

// to determine the time of lifespan of the OTP which we use for reset password or 2 factor authentication for ex
builder.Services.Configure<DataProtectionTokenProviderOptions>(op =>
{
    // otp valid for 1 hours
    op.TokenLifespan = TimeSpan.FromHours(1);
});

// Add services to the container.
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = true; // Optional: for readable JSON
});

builder.Services.AddMemoryCache();

// Add HttpContextAccessor service this is for make func getUserId...
// AddHttpContextAccessor => and this to could us get the token from the header of the API
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// enable token bearer functionality in Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Define the Bearer Authentication scheme
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    // Make sure the bearer token is applied to all requests
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});

var app = builder.Build();

// 1) Create a scope for services
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // 2) Seed roles and initial admin user
        await SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        // You might want to log the exception here.
        Console.WriteLine($"Error seeding roles/admin user: {ex.Message}");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();


// 3) Create the seeding method
static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
{
    // Retrieve RoleManager and UserManager from DI
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Ensure the roles exist
    string[] roleNames = { "admin", "user" };
    foreach (var roleName in roleNames)
    {
        bool roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create an admin user if not exists
    string adminEmail = "admin@example.com";
    string adminPassword = "Admin123#KLJHItuy&*^*8768";     // You can store it in secrets/config.
    var existingAdminUser = await userManager.FindByEmailAsync(adminEmail);

    if (existingAdminUser == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin",     // Could be anything, e.g. "superadmin"
            Email = adminEmail,
            EmailConfirmed = true,  // By default mark it confirmed
            TokenForRessetingPass = ""
        };

        var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createAdminResult.Succeeded)
        {
            // Assign the "admin" role
            await userManager.AddToRoleAsync(adminUser, "admin");
        }
        else
        {
            // Handle creation errors if needed (logging, etc.)
            var errors = string.Join(", ", createAdminResult.Errors.Select(e => e.Description));
            Console.WriteLine("Failed to create admin user: " + errors);
        }
    }
}

