using Application.DTOs.Course;

namespace Application.Features.Courses.Commands.Update
{
    public sealed record UpdateCourseCommand(Guid Id, CourseUpdateDto Dto) : IRequest;
}