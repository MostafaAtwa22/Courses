namespace Infrastructure.Persistence.Configurations
{
    public class ContentConfiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.ToTable("contents");
            
            builder.HasKey(s => s.Id);
            
            builder.Property(c => c.Title)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(c => c.DurationInSeconds)
                .IsRequired();

            builder.Property(c => c.Order)
                .IsRequired();

            builder.HasOne(c => c.Section)
                .WithMany(s => s.Contents)
                .HasForeignKey(c => c.SectionId);

            builder.HasMany(c => c.Files)
                .WithOne(f => f.Content)
                .HasForeignKey(f => f.ContentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
