using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;

namespace Application.Features.Contents.Commands.Delete
{
    public sealed class DeleteContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        IContentAttachmentService _attachmentService,
        IAppCache _cache)
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

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Contents(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Content(request.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Progress(), cancellationToken);
        }
    }
}
