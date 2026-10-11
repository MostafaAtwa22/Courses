using Application.DTOs.Authorization.Permissions;

namespace Application.Common.Interfaces
{
    public interface IPermissionRepository
    {
        Task<PermissionRoleDto> GetRolePermissionsAsync(IdentityRole role, CancellationToken cancellationToken = default);
        Task UpdateRolePermissionsAsync(IdentityRole role, IEnumerable<string> selectedPermissions, CancellationToken cancellationToken = default);
    }
}
