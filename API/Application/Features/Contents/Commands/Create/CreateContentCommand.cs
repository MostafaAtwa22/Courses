using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Contents.Commands.Create
{
    public sealed record CreateContentCommand(ContentCreateDto Dto) : IRequest<Guid>;
}
