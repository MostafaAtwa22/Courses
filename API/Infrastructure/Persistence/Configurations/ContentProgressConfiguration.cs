namespace Infrastructure.Persistence.Configurations
{
    public class ContentProgressConfiguration : IEntityTypeConfiguration<ContentProgress>
    {
        public void Configure(EntityTypeBuilder<ContentProgress> builder)
        {
            builder.ToTable("content_progress");
            
            builder.HasKey(cp => cp.Id);

            builder.HasOne(cp => cp.Student)
                .WithMany(s => s.Progress)
                .HasForeignKey(cp => cp.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.Content)
                .WithMany(c => c.Progress)
                .HasForeignKey(cp => cp.ContentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.Course)
                .WithMany(c => c.Progress)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(cp => cp.CompletedAt)
                .IsRequired();

            builder.HasIndex(cp => new { cp.StudentId, cp.ContentId })
                .IsUnique();

            builder.HasIndex(cp => new { cp.StudentId, cp.CourseId });
        }
    }
}
