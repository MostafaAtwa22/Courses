namespace Application.Features.Authorization.Queries.GetAll;

public sealed record GetRolesQueryHandler(IRoleRepository _roleRepo) :
    IRequestHandler<GetRolesQuery, IReadOnlyCollection<RolesResponseDto>>
{
    public async Task<IReadOnlyCollection<RolesResponseDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await _roleRepo.GetAllRolesAsync(cancellationToken);
    }
}