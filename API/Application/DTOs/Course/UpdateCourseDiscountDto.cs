namespace Application.DTOs.Course
{
    public class UpdateCourseDiscountDto
    {
        public decimal Percentage { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
