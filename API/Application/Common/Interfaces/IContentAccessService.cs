namespace Application.Common.Interfaces
{
    public interface IContentAccessService
    {
        Task<bool> HasFullCourseContentAccessAsync(Guid courseId, CancellationToken cancellationToken = default);
    }
}
