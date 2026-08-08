using Application.Common.Interfaces;

namespace Application.Features.Progress.Commands.MarkIncomplete
{
    public sealed class MarkContentIncompleteCommandHandler(
        IContentProgressRepository _progressRepo)
        : IRequestHandler<MarkContentIncompleteCommand>
    {
        public async Task Handle(MarkContentIncompleteCommand request, CancellationToken cancellationToken)
        {
            await _progressRepo.MarkIncompleteAsync(request.StudentId, request.Dto.ContentId, cancellationToken);
        }
    }
}
