using System.Security.Claims;
using Domain.Enums.Identity;
using Infrastructure.Constants;
using Infrastructure.Enums;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seed.Identity
{
    public static class ApplicationRolePermissionsSeed
    {
        public static async Task SeedAsync(
            RoleManager<IdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
            await SeedSuperAdminAsync(roleManager, logger);
            await SeedAdminAsync(roleManager, logger);
        }

        private static async Task SeedSuperAdminAsync(
            RoleManager<IdentityRole> roleManager,
            ILogger logger)
        {
            var superAdminRole = await roleManager.FindByNameAsync(Role.SuperAdmin.ToString());
            if (superAdminRole is null)
            {
                logger.LogWarning("SuperAdmin role not found. Skipping permission seeding for SuperAdmin.");
                return;
            }

            var permissions = PermissionConstants.GenerateAllPermissions();

            await AddPermissionsAsync(
                roleManager,
                superAdminRole,
                permissions,
                logger
            );
        }

        private static async Task SeedAdminAsync(
            RoleManager<IdentityRole> roleManager,
            ILogger logger)
        {
            var adminRole = await roleManager.FindByNameAsync(Role.Admin.ToString());
            if (adminRole is null)
            {
                logger.LogWarning("Admin role not found. Skipping permission seeding for Admin.");
                return;
            }

            var permissions = PermissionConstants.GenerateAllPermissions()
                .Where(p =>
                    !p.EndsWith(CRUD.Create.ToString()) &&
                    !p.EndsWith(CRUD.Delete.ToString()))
                .ToList();

            await AddPermissionsAsync(
                roleManager,
                adminRole,
                permissions,
                logger
            );
        }

        private static async Task AddPermissionsAsync(RoleManager<IdentityRole> roleManager,
            IdentityRole role,
            IEnumerable<string> permissions,
            ILogger logger)
        {
            var existingClaims = await roleManager.GetClaimsAsync(role);

            var existingPermissions = existingClaims
                .Where(c => c.Type == CustomClaims.Permissions)
                .Select(c => c.Value)
                .ToHashSet();

            foreach (var permission in permissions)
            {
                if (existingPermissions.Contains(permission))
                    continue;

                var result = await roleManager.AddClaimAsync(
                    role,
                    new Claim(CustomClaims.Permissions,
                    permission));

                if (!result.Succeeded)
                {
                    logger.LogError(
                        "Failed to add permission {Permission} to role {Role}. Errors: {Errors}",
                        permission,
                        role.Name,
                        string.Join(
                            ", ",
                            result.Errors.Select(error => error.Description)));

                    continue;
                }

                logger.LogInformation(
                    "Permission {Permission} added to role {Role}.",
                    permission,
                    role.Name);
            }
        }
    }
}
