using Application.Common.Interfaces.Identity;
using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Commands.Update
{
    public sealed record UpdateSectionCommand(Guid Id, SectionUpdateDto Dto) : IRequest, IInstructorOwnedRequest
    {
        public ResourceType ResourceType => ResourceType.Section;
    }
}
