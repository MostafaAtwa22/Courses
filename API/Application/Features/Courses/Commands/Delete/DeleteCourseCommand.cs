
namespace Application.Features.Courses.Commands.Delete
{
    public sealed record DeleteCourseCommand(Guid Id) : IRequest;
}