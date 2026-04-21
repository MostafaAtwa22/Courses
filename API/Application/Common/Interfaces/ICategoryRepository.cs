using Application.DTOs.Category;

namespace Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default);
        Task<CategoryResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}