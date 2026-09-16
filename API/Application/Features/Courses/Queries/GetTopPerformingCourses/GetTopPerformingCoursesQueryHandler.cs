using Application.Common.Interfaces;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetTopPerformingCourses
{
    public sealed class GetTopPerformingCoursesQueryHandler(ICourseRepository _repo)
        : IRequestHandler<GetTopPerformingCoursesQuery, IEnumerable<AdminCourseAnalyticsDto>>
    {
        public Task<IEnumerable<AdminCourseAnalyticsDto>> Handle(GetTopPerformingCoursesQuery request, CancellationToken ct)
        {
            return _repo.GetTopPerformingCoursesAsync(request.Limit, ct);
        }
    }
}
