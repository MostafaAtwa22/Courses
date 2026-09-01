using Application.Common.Interfaces;

namespace Application.Features.Contents.Commands.Delete
{
    public sealed class DeleteContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        IContentAttachmentService _attachmentService)
        : IRequestHandler<DeleteContentCommand>
    {
        public async Task Handle(DeleteContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Content", request.Id);

            // Delete video file
            await _fileService.DeleteAsync(content.ContentUrl);

            // Delete all attachment files
            await _attachmentService.DeleteAllAttachmentsAsync(request.Id, cancellationToken);

            await _repo.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
