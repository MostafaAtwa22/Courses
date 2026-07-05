namespace Infrastructure.Persistence.Configurations;

public class CourseDiscountConfiguration : IEntityTypeConfiguration<CourseDiscount>
{
    public void Configure(EntityTypeBuilder<CourseDiscount> builder)
    {
        builder.ToTable("course_discounts");
            
        builder.HasKey(c => c.Id);
        
        builder.Property(d => d.Percentage)
            .IsRequired()
            .HasPrecision(5, 2)
            .HasDefaultValue(0m);
        
        builder.HasOne(d => d.Course)
            .WithMany(c => c.CourseDiscounts)
            .HasForeignKey(d => d.CourseId);
    }
}