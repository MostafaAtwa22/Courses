namespace Application.Common.Models;

public class StudentQueryParams : QueryParams
{
    public Gender? Gender { get; set; }
    public Guid? CourseId { get; set; }
}
