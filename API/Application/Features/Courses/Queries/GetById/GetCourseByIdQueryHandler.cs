using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetById
{
    public sealed class GetCourseByIdQueryHandler(
        ICourseRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetCourseByIdQuery, CourseResponseDto?>
    {
        public async Task<CourseResponseDto?> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.Course(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(15),
                LocalCacheExpiration: TimeSpan.FromMinutes(5));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetByIdAsync(request.Id, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Courses() },
                cancellationToken);
        }
    }
}