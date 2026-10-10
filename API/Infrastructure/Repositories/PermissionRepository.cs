using Application.Common.Interfaces;
using Application.DTOs.Authorization.Permissions;
using Application.Common.Exceptions;
using Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class PermissionRepository(RoleManager<IdentityRole> roleManager) : IPermissionRepository
    {
        public async Task<PermissionRoleDto> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken)
        {
            var role = await roleManager.FindByIdAsync(roleId)
                ?? throw new NotFoundException(nameof(IdentityRole), Guid.Parse(roleId));

            var claims = await roleManager.GetClaimsAsync(role);
            var rolePermissions = claims
                .Where(c => c.Type == CustomClaims.Permissions)
                .Select(c => c.Value)
                .ToHashSet();

            var allPermissions = PermissionConstants.GenerateAllPermissions();

            return new PermissionRoleDto
            {
                RoleId = Guid.Parse(roleId),
                RoleName = role.Name ?? string.Empty,
                Permissions = [.. allPermissions.Select(p => CreatePermissionCheckbox(p, rolePermissions.Contains(p)))]
            };
        }

        private static PermissionCheckboxDto CreatePermissionCheckbox(string permission, bool isSelected)
        {
            var parts = permission.Split(':');
            return new PermissionCheckboxDto
            {
                Name = permission,
                Module = parts.Length > 1 ? parts[1] : string.Empty,
                Action = parts.Length > 2 ? parts[2] : string.Empty,
                IsSelected = isSelected
            };
        }
    }
}
