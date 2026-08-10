namespace Application.Common.Interfaces
{
    public interface IRequireContentCourseValidation
    {
        Guid CourseId { get; }
        Guid ContentId { get; }
    }
}
