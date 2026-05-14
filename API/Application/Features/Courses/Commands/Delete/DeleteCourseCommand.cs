using Application.Common.Interfaces.Identity;
using MediatR;

namespace Application.Features.Courses.Commands.Delete
{
    public sealed record DeleteCourseCommand(Guid Id) : IRequest, IInstructorOwnedRequest
    {
        public ResourceType ResourceType => ResourceType.Course;
    }
}