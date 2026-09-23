using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetTopPerformingCourses
{
    public sealed class GetTopPerformingCoursesQueryHandler(
        ICourseRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetTopPerformingCoursesQuery, IEnumerable<AdminCourseAnalyticsDto>>
    {
        public async Task<IEnumerable<AdminCourseAnalyticsDto>> Handle(GetTopPerformingCoursesQuery request, CancellationToken ct)
        {
            var cacheKey = CacheKeys.TopPerformingCourses(request.Limit);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(2),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetTopPerformingCoursesAsync(request.Limit, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.Courses(), CacheKeys.AdminDashboard() },
                ct);

            return result ?? Enumerable.Empty<AdminCourseAnalyticsDto>();
        }
    }
}
