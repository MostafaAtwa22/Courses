namespace Application.Features.Courses.Commands.Create
{
    public sealed record CreateCourseCommand(CourseCreateDto Dto) : IRequest<Guid>;
}