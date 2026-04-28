using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetAll
{
    public sealed record GetCoursesQuery(QueryParams QueryParams) : IRequest<PaginatedResult<CourseResponseDto>>;
}