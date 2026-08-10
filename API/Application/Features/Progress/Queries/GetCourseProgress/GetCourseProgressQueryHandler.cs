using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetCourseProgress
{
    public sealed class GetCourseProgressQueryHandler(
        IContentProgressRepository _progressRepo)
        : IRequestHandler<GetCourseProgressQuery, CourseProgressDto>
    {
        public async Task<CourseProgressDto> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
        {
            var summary = await _progressRepo.GetCourseProgressSummaryAsync(request.StudentId, request.CourseId, cancellationToken);
            var completedIds = await _progressRepo.GetCompletedContentIdsAsync(request.StudentId, request.CourseId, cancellationToken);

            return summary.ToDto(completedIds);
        }
    }
}
