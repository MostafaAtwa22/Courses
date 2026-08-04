namespace Application.Common.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<bool> IsEnrolledAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
        Task<bool> IsEnrolledByUserIdAsync(string userId, Guid courseId, CancellationToken cancellationToken = default);
        Task<Guid?> GetCourseIdByContentIdAsync(Guid contentId, CancellationToken cancellationToken = default);
        Task<Guid?> GetInstructorIdByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    }
}
