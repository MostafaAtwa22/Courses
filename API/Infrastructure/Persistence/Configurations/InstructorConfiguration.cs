using Domain.Enums;

namespace Infrastructure.Persistence.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.ToTable("instructors");
            
            builder.HasKey(i => i.Id);
            
            builder.Property(i => i.Bio)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(i => i.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (InstructorStatus)Enum.Parse(typeof(InstructorStatus), s)
                );

            builder.HasOne(i => i.User)
                .WithOne(u => u.InstructorProfile)
                .HasForeignKey<Instructor>(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
