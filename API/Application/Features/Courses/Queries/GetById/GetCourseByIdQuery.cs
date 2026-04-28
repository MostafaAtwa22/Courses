using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetById
{
    public sealed record GetCourseByIdQuery(Guid Id) : IRequest<CourseResponseDto?>;
}