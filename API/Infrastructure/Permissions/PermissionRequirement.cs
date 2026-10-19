using Microsoft.AspNetCore.Authorization;
namespace Infrastructure.Permissions
{
    public class PermissionRequirement(string permission) 
        : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}