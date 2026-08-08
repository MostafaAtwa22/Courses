namespace Application.DTOs.Student;

public class MarkContentCompleteDto
{
    public Guid StudentId { get; set; }
    public Guid ContentId { get; set; }
    public Guid CourseId { get; set; }
}
