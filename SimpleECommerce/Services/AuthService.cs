using SimpleECommerce.DataAndContext.Models;
using SimpleECommerce.Helpers;
using SimpleECommerce.vModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using SimpleECommerce.DataAndContext;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using System.Text.RegularExpressions;
using SimpleECommerce.EmailForms;

namespace SimpleECommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWT _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailSender _emailSender;
        private readonly ITransferPhotosToPathWithStoreService _transferPhotosToPath;
        private readonly ApplicationDbContext _DbContext;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IOptions<JWT> jwtOptions,
            IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            IEmailSender emailSender,
            ITransferPhotosToPathWithStoreService transferPhotosToPath,
            ApplicationDbContext applicationDbContext
            )
        {
            _userManager = userManager;
            _jwt = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _transferPhotosToPath = transferPhotosToPath;
            _DbContext = applicationDbContext;
            _emailSender = emailSender;
        }

        // 2 function to get user Id GetUserIdFromToken(helper), getUserId(main)
        public string getUserId()
        {
            // logic
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("HTTP context is not available.");
            }// Extract the Authorization header
            var authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("Authorization header is missing or invalid.");
            }
            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // Extract the user ID from the token
            return GetUserIdFromToken(token); // Reuse the previously defined methodreturn userId;
        }
        // helping function
        private string GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is required.", nameof(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "uid");

            if (userIdClaim != null)
            {
                return userIdClaim.Value;
            }
            else
            {
                throw new InvalidOperationException("User ID claim ('uid') not found in token.");
            }
        }
        // register
        public async Task<string> sendOtpToConfirmEmailAsync(sendOTPForLoginModel model)
        {

            var user = await _userManager.Users.FirstOrDefaultAsync(n => n.UserName == model.userName);
            if (user != null)
                return "error, this user name is already used for another user!";

            user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == model.email);
            if (user != null)
                return "error, this email is already assigned to another user!";

            return await sendOtpToUserAsync(model.email);
        }
        public async Task<registerResult> registerAsync(userDataModel model)
        {


            var user = await _userManager.Users.FirstOrDefaultAsync(n => n.UserName == model.name);
            if (user != null)
                return new registerResult { Message = "error, this user name is already used for another user!" };

            user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == model.email);
            if (user != null)
                return new registerResult { Message = "error, this Email is already registered for another user!" };

            if (!await IsValidOtpForUserAsync(new VerificationOtp { OTP = model.OTPforEmailConfirmaiton, userEmail = model.email }))
                return new registerResult { Message = "error, OTP is not correct" };

            ApplicationUser newUser = new ApplicationUser
            {
                EmailConfirmed = true,
                UserName = model.name,
                Email = model.email,
                TokenForRessetingPass = ""
            };


            // storing the new user 
            var result = await _userManager.CreateAsync(newUser, model.userPassword);
            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new registerResult { Message = errors };
            }


            // select the user with his full data
            var newUserFullData = await _userManager.Users.FirstOrDefaultAsync(p => p.Email == model.email);


            // creating JWT Token
            var jwtSecurityToken = await CreateJwtTokenAsync(newUserFullData);

            registerResult resultModel = new registerResult
            {
                name = model.name,
                email = model.email,
                Message = "",
                token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo
            };



            return resultModel;
        }
        // login
        public async Task<registerResult> getTokenAsync(loginModel model)
        {
            var authModel = new registerResult();

            var user = await _userManager.FindByEmailAsync(model.userEmail);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.UserPassword))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtTokenAsync(user);

            authModel.token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.email = user.Email;
            authModel.name = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Message = "";
            return authModel;
        }

        // logout
        public string LogOutAsync()
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            BlacklistToken(token);
            return "Logged out successfully";
        }

        // for reseting password for user 2 functions 
        public async Task<string> SendOtpForResetPassAsync(sendOTPForLoginModel model)
        {
            // Check if the user with the provided email exists
            var user = await _DbContext.Users.FirstOrDefaultAsync(u => u.Email == model.email);
            if (user == null || user.UserName != model.userName)
            {
                return "Invalid email or username!";
            }

            // Send OTP to the user's email
            var otpResponse = await sendOtpToUserAsync(model.email);
            if (!string.IsNullOrEmpty(otpResponse))
            {
                return otpResponse;
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Store the token for password resetting
            user.TokenForRessetingPass = token;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return "some thing went wrong please try again.";
            }

            return "";
        }

        public async Task<string> resetPassAsync(resetPassModel model)
        {
            string message = string.Empty;
            var user = await _DbContext.Users.FirstOrDefaultAsync(x => x.Email == model.gmail);
            if (user == null)
            {
                return "invalid user data!";
            }

            if (!await IsValidOtpForUserAsync(new VerificationOtp { OTP = model.OTP, userEmail = model.gmail }))
            {
                return "Invalid or expired OTP.";
            }
            if (model.newPassword != model.confirmNewPassword)
            {
                return "The new password and confirm new password fields do not match.";
            }

            var passwordChanged = await _userManager.ResetPasswordAsync(user, user.TokenForRessetingPass, model.newPassword);
            if (!passwordChanged.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in passwordChanged.Errors)
                {
                    errors.Add(error.Description);
                }
                message = string.Join(", ", errors);
                return message;
            }
            return message;
        }

        // for Addresses Area:-
        // Adding an address for a user
        public async Task<string> AddAddressToUser(AddressInputModel model)
        {
            var userId = getUserId();

            var address = new Address
            {
                UserId = userId,
                BuildingName = model.BuildingName,
                City = model.City,
                Country = model.Country,
                FullName = model.FullName,
                IsDefault = model.IsDefault == null ? false : model.IsDefault,
                MobileNumber = model.MobileNumber,
                StreetName = model.StreetName
            };

            // If the new address is set as default, update other addresses for this user to be non-default
            if (address.IsDefault)
            {
                var addressesForUser = await _DbContext.Addresses
                    .Where(x => x.UserId == userId && x.IsDefault == true)
                    .ToListAsync();

                foreach (var addr in addressesForUser)
                {
                    addr.IsDefault = false;
                }

                _DbContext.UpdateRange(addressesForUser);
            }

            // if user has only one address it must be default
            if (await _DbContext.Addresses.FirstOrDefaultAsync(f => f.UserId == userId) == null)
                address.IsDefault = true;

            // Add the new address
            _DbContext.Addresses.Add(address);
            await _DbContext.SaveChangesAsync();

            return "Address added successfully";
        }
        // Show all addresses for a user
        public async Task<IEnumerable<Address>> ShowUserAddresses()
        {
            var userId = getUserId();
            return await _DbContext.Addresses.Where(a => a.UserId == userId).ToListAsync();
        }

        // Delete a specific address for a user
        public async Task<string> DeleteAddressForUser(int addressId)
        {
            var userId = getUserId();
            var address = await _DbContext.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (address == null) return "Address not found";

            _DbContext.Addresses.Remove(address);

            await _DbContext.SaveChangesAsync();

            // if user has only one address it must be default
            var addresses = await _DbContext.Addresses.Where(a => a.UserId == userId).ToListAsync();
            if (addresses.Count() == 1)
            {
                addresses[0].IsDefault = true;
                await _DbContext.SaveChangesAsync();
            }

            return "Address deleted successfully";
        }

        // Update a specific address for a user
        public async Task<string> UpdateAddress(AddressInputModel model)
        {
            var userId = getUserId();
            var address = await _DbContext.Addresses.FirstOrDefaultAsync(a => a.Id == model.Id && a.UserId == userId);

            if (address == null) return "Address not found";

            address.BuildingName = model.BuildingName;
            address.City = model.City;
            address.Country = model.Country;
            address.FullName = model.FullName;
            address.IsDefault = model.IsDefault;
            address.MobileNumber = model.MobileNumber;
            address.StreetName = model.StreetName;

            if (address.IsDefault)
            {
                var addressesForUser = await _DbContext.Addresses
                    .Where(x => x.UserId == userId && x.IsDefault == true && x.Id != model.Id)
                    .ToListAsync();

                foreach (var addr in addressesForUser)
                {
                    addr.IsDefault = false;
                }

                _DbContext.UpdateRange(addressesForUser);
            }

            await _DbContext.SaveChangesAsync();

            return "Address updated successfully";
        }

        // Set a specific address as default
        public async Task<string> SetAsDefaultAddress(int addressId)
        {
            var userId = getUserId();

            var addressToSetDefault = await _DbContext.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);

            if (addressToSetDefault == null)
            {
                return "Address not found or does not belong to the user.";
            }

            // Unset current default addresses
            var addresses = await _DbContext.Addresses.Where(a => a.UserId == userId).ToListAsync();

            foreach (var addr in addresses)
            {
                addr.IsDefault = addr.Id == addressId;
            }

            await _DbContext.SaveChangesAsync();

            return "Default address updated successfully";
        }

        // Helper method to get the user ID from the token
        // public string getUserId()
        // {
        //     // Assume this method extracts and validates the user ID from the token
        //     return _userManager.GetUserId(_httpContextAccessor.HttpContext.User);
        // }

        // more helping functions

        // for creating Token by JWT
        private async Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? throw new ArgumentNullException(nameof(user.UserName))),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty), // Or throw if required
                new Claim("uid", user.Id ?? throw new ArgumentNullException(nameof(user.Id)))
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        // helping for logout by put generated token in black list
        public void BlacklistToken(string token)
        {
            lock (_blacklistedTokens)
            {
                _blacklistedTokens.Add(token);
            }
        }

        public bool IsTokenBlacklisted(string token)
        {
            lock (_blacklistedTokens)
            {
                return _blacklistedTokens.Contains(token);
            }
        }

        private string generateOtp()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task<string> sendOtpToUserAsync(string userEmail)
        {
            if (userEmail is null || userEmail == "")
                return "error, email for user can't be empty!";

            var otp = generateOtp();
            if (otp is null || otp == "")
                return "error, some thing went wrong please try again!";

            // your logic for storing otps in sql in table phoneOtps
            var newEmailOtp = new EmailOtp
            {
                Otp = otp,
                Email = userEmail,
                ValidTo = DateTime.Now.AddMinutes(5),
            };

            var oldOtp = await _DbContext.emailOtps.FirstOrDefaultAsync(x => x.Email == userEmail);
            if (oldOtp == null)
            {
                // Asynchronously add new OTP entry if no existing one is found
                await _DbContext.emailOtps.AddAsync(newEmailOtp);
            }
            else
            {
                // Update only the necessary fields
                oldOtp.Otp = newEmailOtp.Otp;
                oldOtp.ValidTo = newEmailOtp.ValidTo;
            }

            // Asynchronously save changes to the database
            await _DbContext.SaveChangesAsync();

            // send OTP to user in mail inbox!
            GenerateMail mailGenerator = new GenerateMail();
            var mailBody = mailGenerator.ResetPassOTP(otp);
            await _emailSender.SendEmailAsync(userEmail, "Your one time password(OTP) from SAHM", mailBody);



            return ""; // you should use this if the mailSender is working perffect #
                       //return otp; /////////////////// this modification only and above!!!
        }

        private async Task<bool> IsValidOtpForUserAsync(VerificationOtp request)
        {

            var otpForThisEmail = await _DbContext.emailOtps.FirstOrDefaultAsync(x => x.Email == request.userEmail);

            if ((otpForThisEmail is not null) && (otpForThisEmail.ValidTo > DateTime.Now) && (otpForThisEmail.Otp == request.OTP))
            {
                otpForThisEmail.ValidTo = DateTime.Now;
                await _DbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

    }
}
