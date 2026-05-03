using Application.DTOs.Section;

namespace Application.Common.Interfaces
{
    public interface ISectionRepository
    {
        Task<PaginatedResult<SectionResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default);
        Task<PaginatedResult<SectionResponseDto>> GetByCourseIdAsync(Guid courseId, QueryParams queryParams, CancellationToken ct = default);
        Task<SectionResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Section?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        Task<Guid> CreateAsync(Section section, CancellationToken ct = default);
        Task UpdateAsync(Section section, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}