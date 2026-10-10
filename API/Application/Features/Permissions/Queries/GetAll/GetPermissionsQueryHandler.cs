using Application.Common.Interfaces.Cache;
using Application.DTOs.Authorization.Permissions;

namespace Application.Features.Permissions.Queries.GetAll
{
    public sealed class GetPermissionsQueryHandler(
        IPermissionRepository _permissionRepo,
        IAppCache _cache)
        : IRequestHandler<GetPermissionsQuery, PermissionRoleDto>
    {
        public async Task<PermissionRoleDto> Handle(
            GetPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.PermissionsByRole(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(15),
                LocalCacheExpiration: TimeSpan.FromMinutes(5));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _permissionRepo.GetRolePermissionsAsync(request.Id, ct),
                cacheOptions,
                tags: [CacheKeys.Permissions()],
                cancellationToken);

            return result ?? new PermissionRoleDto { Permissions = [] };
        }
    }
}
