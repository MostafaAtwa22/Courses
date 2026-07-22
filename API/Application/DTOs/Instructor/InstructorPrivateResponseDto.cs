namespace Application.DTOs.Instructor;

public class InstructorPrivateResponseDto : InstructorCommonResponseDto
{
    public string PhoneNumber { get; set; } =  string.Empty;
    public string CvUrl { get; set; } = string.Empty;
}