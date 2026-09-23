using Application.Common.Interfaces.Cache;
using Application.DTOs.Review;
using Domain.Entities.Identity;

namespace Application.Features.Reviews.Queries.GetUserReview
{
    public sealed class GetUserReviewQueryHandler(
        IReviewRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetUserReviewQuery, ReviewResponseDto?>
    {
        public async Task<ReviewResponseDto?> Handle(GetUserReviewQuery request, CancellationToken ct)
        {
            var user = request.User;
            var studentId = await _repo.GetStudentIdByUserIdAsync(user!.Id.ToString(), ct)
                            ?? throw new NotFoundException($"Student", Guid.Parse(user.Id));

            var cacheKey = CacheKeys.ReviewByUserCourse(user.Id.ToString(), request.CourseId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(3),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetByUserAndCourseAsync(studentId, request.CourseId, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.Reviews() },
                ct);
        }
    }
}
