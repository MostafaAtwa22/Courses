using Application.Common.Exceptions;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed class GetCourseProgressQueryHandler(
        IContentProgressRepository _progressRepo,
        ICurrentUserService _currentUserService,
        IAppCache _cache)
        : IRequestHandler<GetCourseProgressQuery, CourseProgressDto>
    {
        public async Task<CourseProgressDto> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");

            var cacheKey = CacheKeys.ProgressByUserCourse(userId, request.CourseId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromSeconds(30),
                LocalCacheExpiration: TimeSpan.FromSeconds(15));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    var summary = await _progressRepo.GetCourseProgressSummaryAsync(request.StudentId, request.CourseId, ct);
                    var completedIds = await _progressRepo.GetCompletedContentIdsAsync(request.StudentId, request.CourseId, ct);

                    return summary.ToDto(completedIds);
                },
                cacheOptions,
                tags: new[] { CacheKeys.Progress() },
                cancellationToken);

            return result ?? new CourseProgressDto();
        }
    }
}
