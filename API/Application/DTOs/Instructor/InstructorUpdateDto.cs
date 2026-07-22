namespace Application.DTOs.Instructor;

public class InstructorUpdateDto : InstructorCommonDto
{
    public IFormFile? CvUrl { get; set; } 
}