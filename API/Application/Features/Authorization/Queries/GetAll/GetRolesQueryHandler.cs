using Application.Common.Interfaces;
using Application.DTOs.Authorization;

namespace Application.Features.Authorization.Queries.GetAll;

public sealed record GetRolesQueryHandler(IRoleRepository _roleRepo) :
    IRequestHandler<GetRolesQuery, IReadOnlyCollection<RoleResponseDto>>
{
    public async Task<IReadOnlyCollection<RoleResponseDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await _roleRepo.GetAllRolesAsync(cancellationToken);
    }
}