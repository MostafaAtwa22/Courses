using Application.DTOs.Category;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<PaginatedResult<CategoryResponseDto>> GetAllAsync(QueryParams queryParams, CancellationToken ct = default);
        Task<CategoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Category?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        Task<Guid> CreateAsync(Category category, CancellationToken ct = default);
        Task UpdateAsync(Category category, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}