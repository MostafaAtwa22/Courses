using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
namespace Infrastructure.Permissions
{
    public class PermissionAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            PermissionRequirement requirement)
        {
            if (context.User.Claims
                .Any(c => c.Type == CustomClaims.Permissions &&
                        c.Value == requirement.Permission))
                context.Succeed(requirement);

            return Task.CompletedTask; 
        }
    }
}