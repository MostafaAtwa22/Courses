using Domain.Enums;

namespace Application.DTOs.Course
{
    public class CoursesResponseDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.InProgress;
        public int Cost { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}