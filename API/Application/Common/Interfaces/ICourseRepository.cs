using Application.DTOs.Course;

namespace Application.Common.Interfaces
{
    public interface ICourseRepository
    {
        Task<PaginatedResult<CoursesResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default!);
        Task<CoursesResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default!);
        Task<Guid> CreateAsync(CourseCreateDto courseCreateDto, CancellationToken ct = default!);
        Task UpdateAsync(Guid id, CourseUpdateDto courseUpdateDto, CancellationToken ct = default!);
        Task DeleteAsync(Guid id, CancellationToken ct = default!);
    }
}