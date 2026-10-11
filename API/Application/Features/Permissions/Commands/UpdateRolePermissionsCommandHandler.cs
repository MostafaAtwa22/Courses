using Application.Common.Interfaces.Cache;
using Application.Common.Exceptions;

namespace Application.Features.Permissions.Commands
{
    public sealed class UpdateRolePermissionsCommandHandler (
        RoleManager<IdentityRole> _roleManager,
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

            await _cache.RemoveByTagAsync(CacheKeys.Permissions(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.PermissionsByRole(request.Id), cancellationToken);
        }
    }
}