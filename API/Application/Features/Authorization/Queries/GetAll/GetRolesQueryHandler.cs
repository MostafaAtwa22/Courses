using Application.Common.Interfaces.Cache;

namespace Application.Features.Authorization.Queries.GetAll;

public sealed record GetRolesQueryHandler(
    IRoleRepository _roleRepo,
    IAppCache _cache) :
    IRequestHandler<GetRolesQuery, IReadOnlyCollection<RolesResponseDto>>
{
    public async Task<IReadOnlyCollection<RolesResponseDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Roles();
        var cacheOptions = new CacheOptions(
            Expiration: TimeSpan.FromMinutes(15),
            LocalCacheExpiration: TimeSpan.FromMinutes(5));

        var result = await _cache.GetOrCreateAsync(
            cacheKey,
            async ct => await _roleRepo.GetAllRolesAsync(ct),
            cacheOptions,
            tags: new[] { CacheKeys.Roles() },
            cancellationToken);
        
        return result ?? [];
    }
}