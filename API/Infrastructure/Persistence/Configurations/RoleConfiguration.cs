using Infrastructure.Constants;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(TableNames.Roles);

            builder.HasKey(r => r.Value);

            builder.Property(r => r.Value)
                .HasColumnName("Id");

            builder
                .HasMany(r => r.Permissions)
                .WithMany()
                .UsingEntity<RolePermission>();
            
            builder.HasMany(r => r.Users)
                .WithMany();
            
            builder.HasData(Role.GetValues());
        }
    }
}