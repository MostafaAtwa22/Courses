using Application.Common.Models;
using Application.DTOs.Admin;
using Domain.Entities.Identity;
using Domain.Enums.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IAdminRepository
{
    Task<AdminResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AdminResponseDto?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<PaginatedResult<AdminResponseDto>> GetAllAsync(AdminQueryParams queryParams, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<bool> IsLastSuperAdminAsync(CancellationToken ct = default);
}