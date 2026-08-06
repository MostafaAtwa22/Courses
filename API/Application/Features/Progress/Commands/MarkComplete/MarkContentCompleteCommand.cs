using Application.DTOs.Progress;

namespace Application.Features.Progress.Commands.MarkComplete
{
    public sealed record MarkContentCompleteCommand(MarkProgressRequestDto Dto) : IRequest;
}
