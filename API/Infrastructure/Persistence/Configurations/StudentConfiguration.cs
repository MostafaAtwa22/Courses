namespace Infrastructure.Persistence.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("students");
            
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Coins)
                .HasDefaultValue(0);

            builder.HasOne(s => s.User)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
