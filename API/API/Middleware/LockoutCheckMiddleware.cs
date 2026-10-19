using System.Security.Claims;
using Application.Common.Interfaces.Cache;
using Microsoft.AspNetCore.Identity;

namespace API.Middleware;

public class LockoutCheckMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IAppCache cache,
        UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            await _next(context);
            return;
        }

        var isLocked = await cache.GetOrCreateAsync(
            CacheKeys.UserLockout(userId),
            async ct =>
            {
                var user = await userManager.FindByIdAsync(userId);
                return user != null && await userManager.IsLockedOutAsync(user);
            },
            new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),        
                LocalCacheExpiration: TimeSpan.FromSeconds(30) 
            ));

        if (isLocked == true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                type = "AccountLocked",
                title = "Account is locked.",
                status = 401
            });
            return;
        }

        await _next(context);
    }
}
