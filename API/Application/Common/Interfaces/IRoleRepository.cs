using Application.DTOs.Authorization;

namespace Application.Common.Interfaces
{
    public interface IRoleRepository
    {
        Task<IReadOnlyCollection<RolesResponseDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<CheckBoxRoleManageDto>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);
    }
}
