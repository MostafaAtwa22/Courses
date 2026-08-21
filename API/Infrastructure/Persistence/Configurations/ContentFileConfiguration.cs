namespace Infrastructure.Persistence.Configurations
{
    public class ContentFileConfiguration : IEntityTypeConfiguration<ContentFile>
    {
        public void Configure(EntityTypeBuilder<ContentFile> builder)
        {
            builder.ToTable("content_files");
            
            builder.HasKey(s => s.Id);
            
            builder.Property(c => c.FileName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(c => c.FileUrl)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne(c => c.Content)
                .WithMany(s => s.Files)
                .HasForeignKey(c => c.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
