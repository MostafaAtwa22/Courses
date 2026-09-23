using Application.Common.Exceptions;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetMyCoursesProgress
{
    public sealed class GetMyCoursesProgressQueryHandler(
        IContentProgressRepository _progressRepo,
        ICurrentUserService _currentUserService,
        IAppCache _cache)
        : IRequestHandler<GetMyCoursesProgressQuery, IReadOnlyList<CourseProgressSummaryDto>>
    {
        public async Task<IReadOnlyList<CourseProgressSummaryDto>> Handle(GetMyCoursesProgressQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");

            var cacheKey = CacheKeys.ProgressByUser(userId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromSeconds(30),
                LocalCacheExpiration: TimeSpan.FromSeconds(15));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _progressRepo.GetMyCoursesProgressAsync(request.StudentId, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Progress() },
                cancellationToken);

            return result ?? Array.Empty<CourseProgressSummaryDto>();
        }
    }
}
