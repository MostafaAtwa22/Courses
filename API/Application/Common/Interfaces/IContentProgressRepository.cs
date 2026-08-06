namespace Application.Common.Interfaces
{
    public interface IContentProgressRepository
    {
        Task<Guid?> GetStudentIdByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task MarkCompleteAsync(Guid studentId, Guid contentId, Guid courseId, CancellationToken cancellationToken = default);
        Task MarkIncompleteAsync(Guid studentId, Guid contentId, CancellationToken cancellationToken = default);
        Task<HashSet<Guid>> GetCompletedContentIdsAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<CourseProgressSummary> GetCourseProgressSummaryAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<CourseProgressSummary>> GetMyCoursesProgressAsync(Guid studentId, CancellationToken cancellationToken = default);
    }

    public class CourseProgressSummary
    {
        public Guid CourseId { get; set; }
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }
        public int PercentComplete { get; set; }
    }
}
