using Application.Common.Interfaces.Cache;
using Application.Common.Exceptions;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Permissions.Commands
{
    public sealed class UpdateRolePermissionsCommandHandler (
        RoleManager<IdentityRole> _roleManager,
        UserManager<ApplicationUser> _userManager,
        IPermissionRepository _permissionRepository,
        IAppCache _cache)
        : IRequestHandler<UpdateRolePermissionsCommand>
    {
        public async Task Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(IdentityRole), Guid.Parse(request.Id));

            var selectedPermissions = request.PermissionsDto
                .Where(p => p.IsSelected)
                .Select(p => p.Name);

            await _permissionRepository.UpdateRolePermissionsAsync(role, selectedPermissions, cancellationToken);

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            foreach (var user in usersInRole)
                await _userManager.UpdateSecurityStampAsync(user);

            await _cache.RemoveByTagAsync(CacheKeys.Permissions(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.PermissionsByRole(request.Id), cancellationToken);
        }
    }
}