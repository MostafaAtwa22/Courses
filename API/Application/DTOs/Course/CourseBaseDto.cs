using Domain.Enums;

namespace Application.DTOs.Course
{
    public class CourseBaseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.InProgress;
        public int Cost { get; set; }
        public Guid CategoryId { get; set; }
    }
}