using Domain.Enums;

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

            builder.Property(c => c.Type)
                .HasConversion(
                    c => c.ToString(),
                    c => (ContentType)Enum.Parse(typeof(ContentType), c)
                );

            builder.Property(c => c.Order)
                .IsRequired();

            builder.HasOne(c => c.Section)
                .WithMany(s => s.Contents)
                .HasForeignKey(c => c.SectionId);
        }
    }
}
