using SimpleECommerce.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SimpleECommerce.middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public JwtMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Create a new scope
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                // Resolve the scoped IAuthService from the scope
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token) && authService.IsTokenBlacklisted(token))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token is blacklisted");
                    return;
                }
            }

            // Proceed with the next middleware
            await _next(context);
        }
    }
}
