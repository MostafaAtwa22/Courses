using Application.DTOs.Authorization;

namespace Application.Common.Interfaces
{
    public interface IRoleRepository
    {
        Task<IReadOnlyCollection<RoleResponseDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    }
}
