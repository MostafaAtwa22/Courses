using Application.DTOs.Category;

namespace Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<PaginatedResult<CategoryResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default);
        Task<CategoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Guid> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default);
        Task UpdateAsync(Guid id, CategoryUpdateDto dto, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}