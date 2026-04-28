using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Course
{
    public class CourseCreateDto : CourseBaseDto
    {
        public IFormFile PictureUrl { get; set; } = default!;
    }
}