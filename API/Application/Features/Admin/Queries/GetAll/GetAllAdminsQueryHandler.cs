using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Admin;

namespace Application.Features.Admin.Queries.GetAll;

public sealed class GetAllAdminsQueryHandler(
    IAdminRepository _repo,
    IAppCache _cache)
    : IRequestHandler<GetAllAdminsQuery, PaginatedResult<AdminResponseDto>>
{
    public async Task<PaginatedResult<AdminResponseDto>> Handle(
        GetAllAdminsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.Params.PageNumber ?? 1;
        var pageSize = request.Params.PageSize ?? 10;
        var searchTerm = request.Params.SearchTerm ?? string.Empty;
        var role = request.Params.Role?.ToString();
        var cacheKey = CacheKeys.Admins(searchTerm, role, pageNumber, pageSize);
        var cacheOptions = new CacheOptions(
            Expiration: TimeSpan.FromMinutes(2),
            LocalCacheExpiration: TimeSpan.FromMinutes(1));

        var result = await _cache.GetOrCreateAsync(
            cacheKey,
            async ct => await _repo.GetAllAsync(request.Params, ct),
            cacheOptions,
            tags: new[] { CacheKeys.Admins() },
            cancellationToken);

        return result ?? new PaginatedResult<AdminResponseDto>([], 0, pageNumber, pageSize);
    }
}