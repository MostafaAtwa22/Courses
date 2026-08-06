using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetMyCoursesProgress
{
    public sealed class GetMyCoursesProgressQueryHandler(
        IContentProgressRepository _progressRepo,
        ICurrentUserService _currentUserService)
        : IRequestHandler<GetMyCoursesProgressQuery, IReadOnlyList<CourseProgressSummaryDto>>
    {
        public async Task<IReadOnlyList<CourseProgressSummaryDto>> Handle(GetMyCoursesProgressQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in to view progress.");

            var studentId = await _progressRepo.GetStudentIdByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only students can view progress.");

            var progress = await _progressRepo.GetMyCoursesProgressAsync(studentId, cancellationToken);

            return progress.Select(p => new CourseProgressSummaryDto
            {
                CourseId = p.CourseId,
                CompletedCount = p.CompletedCount,
                TotalCount = p.TotalCount,
                PercentComplete = p.PercentComplete
            }).ToList();
        }
    }
}
