using Application.DTOs.Progress;

namespace Application.Features.Progress.Commands.MarkIncomplete
{
    public sealed record MarkContentIncompleteCommand(MarkProgressRequestDto Dto) : IRequest;
}
