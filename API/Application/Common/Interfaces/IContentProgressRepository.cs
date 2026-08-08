using Application.DTOs.Progress;

namespace Application.Common.Interfaces
{
    public interface IContentProgressRepository
    {
        Task MarkCompleteAsync(Guid studentId, Guid contentId, Guid courseId, CancellationToken cancellationToken = default);
        Task MarkIncompleteAsync(Guid studentId, Guid contentId, CancellationToken cancellationToken = default);
        Task<HashSet<Guid>> GetCompletedContentIdsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<CourseProgressSummaryDto> GetCourseProgressSummaryAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CourseProgressSummaryDto>> GetMyCoursesProgressAsync(Guid studentId, CancellationToken cancellationToken = default);
    }
}
