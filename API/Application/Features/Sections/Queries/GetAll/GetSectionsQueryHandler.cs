using Application.Common.Interfaces.Cache;
using Application.DTOs.Section;

namespace Application.Features.Sections.Queries.GetAll
{
    public sealed class GetSectionsQueryHandler(
        ISectionRepository _repo,
        IAppCache _cache) : IRequestHandler<GetSectionsQuery, PaginatedResult<SectionResponseDto>>
    {
        public async Task<PaginatedResult<SectionResponseDto>> Handle(GetSectionsQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.QueryParams.PageNumber ?? 1;
            var pageSize = request.QueryParams.PageSize ?? 10;
            var searchTerm = request.QueryParams.SearchTerm ?? string.Empty;
            var cacheKey = CacheKeys.Sections(searchTerm, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(2),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetAllAsync(request.QueryParams, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Sections() },
                cancellationToken);

            return result ?? new PaginatedResult<SectionResponseDto>([], 0, pageNumber, pageSize);
        }
    }
}
