using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Admin;

namespace Application.Features.Admin.Queries.GetById;

public sealed class GetAdminByIdQueryHandler(
    IAdminRepository _repo,
    IAppCache _cache)
    : IRequestHandler<GetAdminByIdQuery, AdminResponseDto?>
{
    public async Task<AdminResponseDto?> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Admin(request.Id);
        var cacheOptions = new CacheOptions(
            Expiration: TimeSpan.FromMinutes(2),
            LocalCacheExpiration: TimeSpan.FromMinutes(1));

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async ct => await _repo.GetByIdAsync(request.Id, ct),
            cacheOptions,
            tags: new[] { CacheKeys.Admins() },
            cancellationToken);
    }
}