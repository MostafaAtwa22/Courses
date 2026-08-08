namespace Application.Features.Progress.Commands.MarkComplete
{
    public sealed class MarkContentCompleteCommandHandler(
        IContentProgressRepository _progressRepo)
        : IRequestHandler<MarkContentCompleteCommand>
    {
        public async Task Handle(MarkContentCompleteCommand request, CancellationToken cancellationToken)
        {
            await _progressRepo.MarkCompleteAsync(request.StudentId, request.Dto.ContentId, request.Dto.CourseId, cancellationToken);
        }
    }
}
