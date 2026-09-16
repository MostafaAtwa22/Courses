using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetTopPerformingCourses
{
    public sealed record GetTopPerformingCoursesQuery(int Limit = 5) : IRequest<IEnumerable<AdminCourseAnalyticsDto>>;
}
