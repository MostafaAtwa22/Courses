using Application.Common.Interfaces.Identity;
using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Contents.Commands.Update
{
    public sealed record UpdateContentCommand(Guid Id, ContentUpdateDto Dto) : IRequest, IInstructorOwnedRequest
    {
        public ResourceType ResourceType => ResourceType.Content;
    }
}
