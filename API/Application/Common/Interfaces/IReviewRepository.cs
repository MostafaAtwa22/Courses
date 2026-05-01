using Application.DTOs.Review;

namespace Application.Common.Interfaces
{
    public interface IReviewRepository
    {
        Task<PaginatedResult<ReviewResponseDto>> GetByCourseAsync(Guid courseId, QueryParams queryParams, CancellationToken ct = default!);
        Task<ReviewResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default!);
        Task<Review?> GetEntityByIdAsync(Guid id, CancellationToken ct = default!);
        Task<bool> IsStudentEnrolledAsync(Guid studentId, Guid courseId, CancellationToken ct = default!);
        Task<bool> HasStudentReviewedAsync(Guid studentId, Guid courseId, CancellationToken ct = default!);
        Task<Guid?> GetStudentIdByUserIdAsync(string userId, CancellationToken ct = default!);
        Task<Guid> CreateAsync(Review review, CancellationToken ct = default!);
        Task UpdateAsync(Review review, CancellationToken ct = default!);
        Task DeleteAsync(Guid id, CancellationToken ct = default!);
    }
}
