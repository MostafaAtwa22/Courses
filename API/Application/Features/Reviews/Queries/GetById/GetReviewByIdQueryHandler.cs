using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Review;

namespace Application.Features.Reviews.Queries.GetById
{
    public sealed class GetReviewByIdQueryHandler(
        IReviewRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetReviewByIdQuery, ReviewResponseDto?>
    {
        public async Task<ReviewResponseDto?> Handle(GetReviewByIdQuery request, CancellationToken ct)
        {
            var cacheKey = CacheKeys.Review(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetByIdAsync(request.Id, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.Reviews() },
                ct);
        }
    }
}
