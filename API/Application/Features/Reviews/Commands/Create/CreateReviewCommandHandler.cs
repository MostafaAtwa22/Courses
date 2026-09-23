using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Mappings;

namespace Application.Features.Reviews.Commands.Create
{
    public sealed class CreateReviewCommandHandler(
        IReviewRepository _repo,
        ICurrentUserService _currentUserService,
        IAppCache _cache)
        : IRequestHandler<CreateReviewCommand, Guid>
    {
        public async Task<Guid> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to leave a review.");

            var studentId = await _repo.GetStudentIdByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only students can leave reviews.");

            var isEnrolled = await _repo.IsStudentEnrolledAsync(
                studentId, request.Dto.CourseId, cancellationToken);

            if (!isEnrolled)
                throw new ForbiddenException("You must be enrolled in this course to leave a review.");

            var alreadyReviewed = await _repo.HasStudentReviewedAsync(
                studentId, request.Dto.CourseId, cancellationToken);

            if (alreadyReviewed)
                throw new ConflictException("You have already submitted a review for this course.");

            var review = request.Dto.ToEntity();
            review.StudentId = studentId;

            var reviewId = await _repo.CreateAsync(review, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Reviews(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Course(request.Dto.CourseId), cancellationToken);

            return reviewId;
        }
    }
}
