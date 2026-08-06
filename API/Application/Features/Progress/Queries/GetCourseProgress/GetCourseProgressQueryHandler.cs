using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed class GetCourseProgressQueryHandler(
        IContentProgressRepository _progressRepo,
        IEnrollmentRepository _enrollmentRepo,
        ICurrentUserService _currentUserService)
        : IRequestHandler<GetCourseProgressQuery, CourseProgressDto>
    {
        public async Task<CourseProgressDto> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in to view progress.");

            var studentId = await _progressRepo.GetStudentIdByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only students can view progress.");

            var isEnrolled = await _enrollmentRepo.IsEnrolledByUserIdAsync(userId, request.CourseId, cancellationToken);
            if (!isEnrolled)
                throw new ForbiddenException("You must be enrolled in this course to view progress.");

            var summary = await _progressRepo.GetCourseProgressSummaryAsync(studentId, request.CourseId, cancellationToken);
            var completedIds = await _progressRepo.GetCompletedContentIdsAsync(studentId, request.CourseId, cancellationToken);

            return new CourseProgressDto
            {
                CourseId = summary.CourseId,
                CompletedCount = summary.CompletedCount,
                TotalCount = summary.TotalCount,
                PercentComplete = summary.PercentComplete,
                CompletedContentIds = completedIds.ToList()
            };
        }
    }
}
