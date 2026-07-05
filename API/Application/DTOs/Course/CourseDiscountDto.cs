namespace Application.DTOs.Course
{
    public class CourseDiscountDto
    {
        public Guid Id { get; set; }
        public decimal Percentage { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public bool IsActive { get; set; }
        public Guid CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
