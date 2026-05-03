using Application.Common.Models;
using Application.DTOs.Content;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface IContentRepository
    {
        Task<IReadOnlyList<ContentResponseDto>> GetBySectionAsync(Guid sectionId, CancellationToken ct = default);
        Task<PaginatedResult<ContentResponseDto>> GetByCourseAsync(Guid courseId, QueryParams queryParams, CancellationToken ct = default);
        Task<ContentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Content?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        Task<Guid> CreateAsync(Content content, CancellationToken ct = default);
        Task UpdateAsync(Content content, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
