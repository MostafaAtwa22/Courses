using MediatR;

namespace Application.Features.Sections.Commands.Delete
{
    public sealed record DeleteSectionCommand(Guid Id) : IRequest;
}
