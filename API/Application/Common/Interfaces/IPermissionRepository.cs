using Application.DTOs.Authorization.Permissions;

namespace Application.Common.Interfaces
{
    public interface IPermissionRepository
    {
        Task<PermissionRoleDto> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken = default);
    }
}
