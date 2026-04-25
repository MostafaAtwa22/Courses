using Application.DTOs.Course;

namespace Application.Common.Interfaces
{
    public interface ICourseRepository
    {
        Task<PaginatedResult<CoursesResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default!);
        Task<CoursesResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default!);
    }
}