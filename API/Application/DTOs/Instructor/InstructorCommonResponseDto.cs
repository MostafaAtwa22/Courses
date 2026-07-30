namespace Application.DTOs.Instructor;

public class InstructorCommonResponseDto : BaseUserResponseDto
{        
    public string Bio { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string LinkedInProfileUrl { get; set; } = string.Empty;
    public string GitHubProfileUrl { get; set; } = string.Empty;
    public InstructorStatus Status { get; set; } = InstructorStatus.Pending;
    public double AverageRate { get; set; }
    public int TotalReviews { get; set; }
    public int TotalStudents { get; set; }
    public int TotalCourses { get; set; }
}