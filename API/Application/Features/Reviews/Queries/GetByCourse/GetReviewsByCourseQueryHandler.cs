using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Review;

namespace Application.Features.Reviews.Queries.GetByCourse
{
    public sealed class GetReviewsByCourseQueryHandler(
        IReviewRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetReviewsByCourseQuery, PaginatedResult<ReviewResponseDto>>
    {
        public async Task<PaginatedResult<ReviewResponseDto>> Handle(GetReviewsByCourseQuery request, CancellationToken ct)
        {
            var pageNumber = request.QueryParams.PageNumber ?? 1;
            var pageSize = request.QueryParams.PageSize ?? 10;
            var cacheKey = CacheKeys.ReviewsByCourse(request.CourseId, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetByCourseAsync(request.CourseId, request.QueryParams, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.Reviews(), CacheKeys.Courses() },
                ct);

            return result ?? new PaginatedResult<ReviewResponseDto>([], 0, pageNumber, pageSize);
        }
    }
}
