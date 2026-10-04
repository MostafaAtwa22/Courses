using Infrastructure.Constants;
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
            HashSet<string> permissions = [.. context.User.Claims
                .Where(c => c.Type == PermissionConstants.Permissions)
                .Select(c => c.Value)];
            
            if (permissions.Contains(requirement.Permission))
                context.Succeed(requirement);

            return Task.CompletedTask; 
        }
    }
}