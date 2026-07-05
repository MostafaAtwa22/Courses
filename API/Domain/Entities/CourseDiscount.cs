namespace Domain.Entities;

public class CourseDiscount : BaseEntity
{
    public decimal Percentage { get; set; }
    public DateTimeOffset StartTime { get; set; }
    public DateTimeOffset EndTime { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = default!;
}