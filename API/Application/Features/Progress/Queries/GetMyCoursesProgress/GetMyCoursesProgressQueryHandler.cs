using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetMyCoursesProgress
{
    public sealed class GetMyCoursesProgressQueryHandler(
        IContentProgressRepository _progressRepo)
        : IRequestHandler<GetMyCoursesProgressQuery, IReadOnlyList<CourseProgressSummaryDto>>
    {
        public async Task<IReadOnlyList<CourseProgressSummaryDto>> Handle(GetMyCoursesProgressQuery request, CancellationToken cancellationToken)
        {
            return await _progressRepo.GetMyCoursesProgressAsync(request.StudentId, cancellationToken);
        }
    }
}
