namespace Application.Features.Authorization.Queries.GetRoleByUserId;

public sealed record GetRoleByUserIdQueryHandler(
    IRoleRepository _roleRepo,
    UserManager<ApplicationUser> _userManager) :
    IRequestHandler<GetRoleByUserIdQuery, UserRolesResponseDto>
{
    public async Task<UserRolesResponseDto> Handle(GetRoleByUserIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(request.UserId));

        var roles = await _roleRepo.GetUserRolesAsync(request.UserId, cancellationToken);

        return user.ToUserRolesManageDto(roles);
    }
}