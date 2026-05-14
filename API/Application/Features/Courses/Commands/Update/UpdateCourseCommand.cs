using Application.Common.Interfaces.Identity;
using Application.DTOs.Course;
using MediatR;

namespace Application.Features.Courses.Commands.Update
{
    public sealed record UpdateCourseCommand(Guid Id, CourseUpdateDto Dto) : IRequest, IInstructorOwnedRequest
    {
        public ResourceType ResourceType => ResourceType.Course;
    }
}