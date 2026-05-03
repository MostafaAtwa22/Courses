using MediatR;

namespace Application.Features.Contents.Commands.Delete
{
    public sealed record DeleteContentCommand(Guid Id) : IRequest;
}
