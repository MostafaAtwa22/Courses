namespace Application.Common.Interfaces.Identity
{
    public interface IRequireEnrollment
    {
        Guid CourseId { get; }
        Guid ContentId { get; }
        bool AllowPreview { get; }
    }
}
