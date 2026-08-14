namespace Application.Features.Courses.Queries.GetByInstructorIdPublic
{
    public sealed record GetCoursesByInstructorIdPublicQuery(Guid InstructorId, CourseQueryParams QueryParams) 
        : IRequest<PaginatedResult<CourseSummaryDto>>;
}
