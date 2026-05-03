namespace Infrastructure.Persistence.Configurations
{
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.ToTable("sections");
            
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Title)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(s => s.Order)
                .IsRequired();

            builder.HasOne(s => s.Course)
                .WithMany(c => c.Sections)
                .HasForeignKey(s => s.CourseId);
        }
    }
}
