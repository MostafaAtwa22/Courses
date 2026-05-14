using Application.Common.Interfaces.Identity;
using MediatR;

namespace Application.Features.Sections.Commands.Delete
{
    public sealed record DeleteSectionCommand(Guid Id) : IRequest, IInstructorOwnedRequest
    {
        public ResourceType ResourceType => ResourceType.Section;
    }
}
