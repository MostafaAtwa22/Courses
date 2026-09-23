using Application.Common.Interfaces.Cache;
using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetBySection
{
    public sealed class GetContentBySectionQueryHandler(
        IContentRepository _repo,
        IAppCache _cache) 
        : IRequestHandler<GetContentBySectionQuery, IReadOnlyList<ContentResponseDto>>
    {
        public async Task<IReadOnlyList<ContentResponseDto>> Handle(GetContentBySectionQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.ContentsBySection(request.SectionId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetBySectionAsync(request.SectionId, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Contents(), CacheKeys.Sections() },
                cancellationToken);
            
            return result ?? [];
        }
    }
}
