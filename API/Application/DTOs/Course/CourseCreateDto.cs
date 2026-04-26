using Domain.Enums;

namespace Application.DTOs.Course
{
    public class CourseCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.InProgress;
        public int Cost { get; set; }
        public Guid CategoryId { get; set; }
    }
}