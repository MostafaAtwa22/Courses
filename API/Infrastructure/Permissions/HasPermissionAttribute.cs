using Domain.Constants;
using Domain.Enums.Identity;
using Microsoft.AspNetCore.Authorization;
namespace Infrastructure.Permissions
{
    public sealed class HasPermissionAttribute(Module module, CRUD action)
        : AuthorizeAttribute(policy: PermissionConstants.Build(module, action))
    {
    }
}