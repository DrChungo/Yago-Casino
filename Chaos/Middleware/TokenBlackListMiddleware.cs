using Chaos.Api.Interface;
using System.Security.Claims;

namespace Chaos.Api.Middleware
{
    public class TokenBlackListMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, ITokenBlackListService blacklistService, IUserService userService)
        {
            var token = context.Request.Headers.Authorization.ToString()
                              .Replace("Bearer ", "");

            //Verificar token revocado (ya lo tenías)
            if (!string.IsNullOrEmpty(token) && blacklistService.IsTokenRevoked(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token revoked. Your session has been closed.");
                return;
            }

            //Verificar IsActive del usuario
            var userIdString = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out Guid userId))
            {
                var user = userService.GetUserById(userId);
                if (user != null && !user.IsAlive)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Account is disabled.");
                    return;
                }
            }

            await _next(context);
        }
    }
}