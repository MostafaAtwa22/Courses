using Microsoft.AspNetCore.Authorization;
using Permission = Domain.Enums.Identity.Permission;
namespace Infrastructure.Permissions
{
    public class HasPermissionAttribute(Permission permission) 
        : AuthorizeAttribute(policy: permission.ToString())
    {
    }
}