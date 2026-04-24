using Domain.Enums;

namespace Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("courses");
            
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(c => c.PictureUrl)
                .HasMaxLength(500);


            builder.Property(c => c.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (CourseStatus)Enum.Parse(typeof(CourseStatus), s)
                );

            builder.HasOne(c => c.Category)
                .WithMany(ca => ca.Courses)
                .HasForeignKey(c => c.CategoryId);
        }
    }
}