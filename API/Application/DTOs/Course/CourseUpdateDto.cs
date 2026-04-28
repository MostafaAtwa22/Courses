namespace Application.DTOs.Course
{
    public class CourseUpdateDto : CourseBaseDto
    {
        public IFormFile PictureUrl { get; set; } = default!;
    }
}