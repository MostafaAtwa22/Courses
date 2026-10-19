using Application.Common.Interfaces.Cache;
using Application.DTOs.Authorization.Permissions;

namespace Application.Features.Permissions.Queries.GetAll
{
    public sealed class GetPermissionsQueryHandler(
        IPermissionRepository _permissionRepo,
        RoleManager<IdentityRole> _roleManager,
        IAppCache _cache)
        : IRequestHandler<GetPermissionsQuery, PermissionRoleDto>
    {
        public async Task<PermissionRoleDto> Handle(
            GetPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id)
                ?? throw new NotFoundException(nameof(IdentityRole), Guid.Parse(request.Id));

            var cacheKey = CacheKeys.PermissionsByRole(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(15),
                LocalCacheExpiration: TimeSpan.FromMinutes(5));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _permissionRepo.GetRolePermissionsAsync(role, ct),
                cacheOptions,
                tags: [CacheKeys.Permissions()],
                cancellationToken);

            return result ?? new PermissionRoleDto { Permissions = [] };
        }
    }
}
