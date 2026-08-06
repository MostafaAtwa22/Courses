using Application.DTOs.Progress;

namespace Application.Features.Progress.Queries.GetMyCoursesProgress
{
    public sealed record GetMyCoursesProgressQuery : IRequest<IReadOnlyList<CourseProgressSummaryDto>>;
}
