using Infrastructure.Constants;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable(TableNames.RolePermissions);

            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            var allPermissions = Enum.GetValues<Domain.Enums.Identity.Permission>();

            var superAdminPermissions = allPermissions
                .Select(p => Create(Role.SuperAdmin, p))
                .ToArray();
            
            var adminPermissions = allPermissions
                .Where(permission =>
                    permission != Domain.Enums.Identity.Permission.CreateAdmin &&
                    permission != Domain.Enums.Identity.Permission.DeleteAdmin &&
                    permission != Domain.Enums.Identity.Permission.LockingUser && 
                    permission != Domain.Enums.Identity.Permission.UpdateRole)
                .Select(permission =>
                    Create(Role.Admin, permission))
                .ToArray();
            
            builder.HasData([.. superAdminPermissions, .. adminPermissions]);
        }
        private static RolePermission Create(Role role, Domain.Enums.Identity.Permission permission)
        {
            return new RolePermission
            {
                RoleId = role.Value,
                PermissionId = (int)permission
            };
        }
    }
}