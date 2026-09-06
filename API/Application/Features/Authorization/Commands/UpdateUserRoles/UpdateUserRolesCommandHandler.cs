namespace Application.Features.Authorization.Commands.UpdateUserRoles;

public sealed class UpdateUserRolesCommandHandler(
    UserManager<ApplicationUser> _userManager) : IRequestHandler<UpdateUserRolesCommand>
{
    public async Task Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId) 
                ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(request.UserId));

        var currentUserRoles = await _userManager.GetRolesAsync(user);
        var selectedRoles = request.Dto.Roles
            .Where(r => r.IsSelected)
            .Select(r => r.RoleName)
            .ToList();

        var rolesToAdd = selectedRoles.Except(currentUserRoles).ToList();
        var rolesToRemove = currentUserRoles.Except(selectedRoles).ToList();

        if (rolesToRemove.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
                throw new BadRequestException(removeResult.Errors.Select(e => e.Description));
        }

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                throw new BadRequestException(addResult.Errors.Select(e => e.Description));
        }
    }
}