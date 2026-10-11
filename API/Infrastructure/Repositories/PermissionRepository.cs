using Application.DTOs.Authorization.Permissions;
using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Infrastructure.Repositories
{
    public class PermissionRepository(RoleManager<IdentityRole> roleManager) : IPermissionRepository
    {
        public async Task<PermissionRoleDto> GetRolePermissionsAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            var rolePermissions = claims
                .Where(c => c.Type == CustomClaims.Permissions)
                .Select(c => c.Value)
                .ToHashSet();

            var allPermissions = PermissionConstants.GenerateAllPermissions();

            return new PermissionRoleDto
            {
                RoleId = Guid.Parse(role.Id),
                RoleName = role.Name ?? string.Empty,
                Permissions = [.. allPermissions.Select(p => CreatePermissionCheckbox(p, rolePermissions.Contains(p)))]
            };
        }

        public async Task UpdateRolePermissionsAsync(IdentityRole role, IEnumerable<string> selectedPermissions, CancellationToken cancellationToken = default)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            var currentPermissions = claims
                .Where(c => c.Type == CustomClaims.Permissions)
                .Select(c => c.Value)
                .ToHashSet();

            var selectedPermissionsSet = selectedPermissions.ToHashSet();

            var permissionsToAdd = selectedPermissionsSet.Except(currentPermissions).ToList();
            var permissionsToRemove = currentPermissions.Except(selectedPermissionsSet).ToList();

            foreach (var permission in permissionsToRemove)
            {
                var claim = claims.FirstOrDefault(c => c.Type == CustomClaims.Permissions && c.Value == permission);
                if (claim != null)
                {
                    var result = await roleManager.RemoveClaimAsync(role, claim);
                    if (!result.Succeeded)
                        throw new InvalidOperationException($"Failed to remove the permission `{permission}` from the role: {role.Name}");
                }
            }

            foreach (var permission in permissionsToAdd)
            {
                var result = await roleManager.AddClaimAsync(
                    role,
                    new Claim(CustomClaims.Permissions, permission)
                );

                if (!result.Succeeded)
                    throw new InvalidOperationException($"Failed to add the permission `{permission}` to the role: {role.Name}");
            }
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
