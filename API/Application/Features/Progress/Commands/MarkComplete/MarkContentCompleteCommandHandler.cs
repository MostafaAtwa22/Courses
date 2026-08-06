using Application.Common.Exceptions;
using Application.Common.Interfaces;

namespace Application.Features.Progress.Commands.MarkComplete
{
    public sealed class MarkContentCompleteCommandHandler(
        IContentProgressRepository _progressRepo,
        IEnrollmentRepository _enrollmentRepo,
        ICurrentUserService _currentUserService)
        : IRequestHandler<MarkContentCompleteCommand>
    {
        public async Task Handle(MarkContentCompleteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in to mark content as complete.");

            var studentId = await _progressRepo.GetStudentIdByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only students can mark content as complete.");

            var isEnrolled = await _enrollmentRepo.IsEnrolledByUserIdAsync(userId, request.Dto.CourseId, cancellationToken);
            if (!isEnrolled)
                throw new ForbiddenException("You must be enrolled in this course to track progress.");

            // Verify content belongs to course
            var courseIdForContent = await _enrollmentRepo.GetCourseIdByContentIdAsync(request.Dto.ContentId, cancellationToken);
            if (courseIdForContent != request.Dto.CourseId)
                throw new BadRequestException("Content does not belong to the specified course.");

            await _progressRepo.MarkCompleteAsync(studentId, request.Dto.ContentId, request.Dto.CourseId, cancellationToken);
        }
    }
}
