namespace Application.Features.Courses.Queries.GetByStudentId
{
    public sealed record GetCoursesByStudentIdQuery(CourseQueryParams QueryParams) : IRequest<PaginatedResult<CourseSummaryDto>>;
}
