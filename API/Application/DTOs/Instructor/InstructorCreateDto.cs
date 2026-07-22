namespace Application.DTOs.Instructor;

public class InstructorCreateDto : InstructorCommonDto
{
    public IFormFile CvUrl { get; set; } = default!;
}