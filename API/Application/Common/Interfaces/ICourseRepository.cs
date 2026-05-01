using Application.DTOs.Course;

namespace Application.Common.Interfaces
{
    public interface ICourseRepository
    {
        Task<PaginatedResult<CourseResponseDto>> GetAllAsync(CourseQueryParams queryParams, CancellationToken ct = default!);
        Task<CourseResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default!);
        Task<Course?> GetEntityByIdAsync(Guid id, CancellationToken ct = default!);
        Task<Guid> CreateAsync(Course course, CancellationToken ct = default!);
        Task UpdateAsync(Course course, CancellationToken ct = default!);
        Task DeleteAsync(Guid id, CancellationToken ct = default!);
        Task<IEnumerable<string>> GetSuggestionsAsync(string term, CancellationToken ct = default!);
    }
}