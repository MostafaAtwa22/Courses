namespace Infrastructure.Persistence.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("reviews");

            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.Headline)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(r => new { r.StudentId, r.CourseId })
                .IsUnique();

            builder.Property(c => c.Rating)
                .HasColumnType("numeric(3, 1)")
                .IsRequired();

            builder.HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId);

            builder.HasOne(r => r.Student)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.StudentId);
        }
    }
}
