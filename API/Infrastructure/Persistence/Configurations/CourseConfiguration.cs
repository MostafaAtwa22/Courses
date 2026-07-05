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
            
            builder.Property(c => c.Cost)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);
            
            builder.Property(c => c.PictureUrl)
                .HasMaxLength(500);

            builder.Property(c => c.AverageRate)
                .HasColumnType("numeric(3, 1)")
                .IsRequired()
                .HasDefaultValue(0m);
        
            builder.Property(c => c.TotalReviews)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.StudentCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.Language)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue("English");

            builder.Property(c => c.WhatYouWillLearn)
                .IsRequired();

            builder.Property(c => c.Requirements)
                .IsRequired();

            builder.Property(c => c.IntroVideoUrl)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(c => c.Status)
                .HasConversion(
                    s => s.ToString(),
                    s => (CourseStatus)Enum.Parse(typeof(CourseStatus), s)
                );

            builder.HasOne(c => c.Category)
                .WithMany(ca => ca.Courses)
                .HasForeignKey(c => c.CategoryId);

            builder.HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}