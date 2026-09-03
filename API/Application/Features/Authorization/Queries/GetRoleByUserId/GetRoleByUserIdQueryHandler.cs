namespace Application.Features.Authorization.Queries.GetAll;

public sealed record GetRoleByUserIdQueryHandler(
    IRoleRepository _roleRepo,
    UserManager<ApplicationUser> _userManager) :
    IRequestHandler<GetRoleByUserIdQuery, UserRolesManageDto>
{
    public async Task<UserRolesManageDto> Handle(GetRoleByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(request.UserId));

        var roles = await _roleRepo.GetUserRolesAsync(request.UserId, cancellationToken);

        return user.ToUserRolesManageDto(roles);
    }
}