using Application.DTOs.Authorization.Permissions;
using Domain.Constants;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories
{
    public class PermissionRepository(
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext dbContext) : IPermissionRepository
    {
        public async Task<PermissionRoleDto> GetRolePermissionsAsync(IdentityRole role, CancellationToken cancellationToken = default)
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
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await dbContext.RoleClaims
                    .Where(rc => rc.RoleId == role.Id && rc.ClaimType == CustomClaims.Permissions)
                    .ExecuteDeleteAsync(cancellationToken);

                // Bulk insert the new ones
                var newClaims = selectedPermissions.Select(p => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = CustomClaims.Permissions,
                    ClaimValue = p
                }).ToList();

                if (newClaims.Count > 0)
                {
                    await dbContext.RoleClaims.AddRangeAsync(newClaims, cancellationToken);
                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
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
