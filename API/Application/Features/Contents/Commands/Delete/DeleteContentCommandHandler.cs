namespace Application.Features.Contents.Commands.Delete
{
    public sealed class DeleteContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        IContentFileRepository _contentFileRepo)
        : IRequestHandler<DeleteContentCommand>
    {
        public async Task Handle(DeleteContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Content", request.Id);

            // Delete video file
            await _fileService.DeleteAsync(content.ContentUrl);

            // Delete all attachment files
            var attachments = await _contentFileRepo.GetByContentIdAsync(request.Id, cancellationToken);
            foreach (var attachment in attachments)
            {
                await _fileService.DeleteAsync(attachment.FileUrl);
                await _contentFileRepo.DeleteAsync(attachment.Id, cancellationToken);
            }

            await _repo.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
