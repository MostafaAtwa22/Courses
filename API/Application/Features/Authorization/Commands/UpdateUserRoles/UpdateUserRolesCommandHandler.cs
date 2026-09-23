using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Domain.Enums.Identity;

namespace Application.Features.Authorization.Commands.UpdateUserRoles;

public sealed class UpdateUserRolesCommandHandler(
    UserManager<ApplicationUser> _userManager,
    IStudentProfileService _studentProfileService,
    IInstructorProfileService _instructorProfileService,
    IAppCache _cache) : IRequestHandler<UpdateUserRolesCommand>
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

            if (rolesToRemove.Contains(Role.Student.ToString()))
                await _studentProfileService.RemoveStudentProfileAsync(user.Id, cancellationToken);

            if (rolesToRemove.Contains(Role.Instructor.ToString()))
                await _instructorProfileService.RemoveInstructorProfileAsync(user.Id, cancellationToken);
        }

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
                throw new BadRequestException(addResult.Errors.Select(e => e.Description));

            if (rolesToAdd.Contains(Role.Student.ToString()))
                await _studentProfileService.EnsureStudentProfileAsync(user.Id, cancellationToken);
        }

        // Invalidate related caches
        await _cache.RemoveByTagAsync(CacheKeys.Users(), cancellationToken);
        await _cache.RemoveByTagAsync(CacheKeys.Roles(), cancellationToken);
        await _cache.RemoveAsync(CacheKeys.RoleByUser(request.UserId), cancellationToken);
    }
}