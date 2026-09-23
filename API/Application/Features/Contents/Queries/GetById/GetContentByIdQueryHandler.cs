using Application.Common.Interfaces.Cache;
using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetById
{
    public sealed class GetContentByIdQueryHandler(
        IContentRepository _repo,
        IAppCache _cache) 
        : IRequestHandler<GetContentByIdQuery, ContentResponseDto?>
    {
        public async Task<ContentResponseDto?> Handle(GetContentByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.Content(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetByIdAsync(request.Id, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Contents() },
                cancellationToken);
        }
    }
}
