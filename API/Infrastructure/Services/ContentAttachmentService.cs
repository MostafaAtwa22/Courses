using Application.Common.Exceptions;
using Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class ContentAttachmentService(
        IFileService _fileService,
        IContentFileRepository _contentFileRepo)
        : IContentAttachmentService
    {
        public async Task<List<ContentFile>> UploadAttachmentsAsync(Guid contentId, List<IFormFile> attachments, CancellationToken cancellationToken)
        {
            var contentFiles = new List<ContentFile>();
            var attachmentFolder = $"{FolderPaths.contentAttachments}/{contentId}";

            foreach (var attachment in attachments)
            {
                var attachmentUrl = await _fileService.UploadAsync(attachment.OpenReadStream(), attachment.FileName, attachmentFolder);
                var contentFile = new ContentFile
                {
                    Id = Guid.NewGuid(),
                    FileName = attachment.FileName,
                    FileUrl = attachmentUrl,
                    ContentId = contentId,
                    CreatedAt = DateTime.UtcNow
                };
                await _contentFileRepo.CreateAsync(contentFile, cancellationToken);
                contentFiles.Add(contentFile);
            }

            return contentFiles;
        }

        public async Task DeleteAttachmentsAsync(List<Guid> attachmentIds, CancellationToken cancellationToken)
        {
            foreach (var attachmentId in attachmentIds)
            {
                var attachment = await _contentFileRepo.GetByIdAsync(attachmentId, cancellationToken);
                if (attachment is not null)
                {
                    await _fileService.DeleteAsync(attachment.FileUrl);
                    await _contentFileRepo.DeleteAsync(attachmentId, cancellationToken);
                }
            }
        }

        public async Task ValidateAttachmentCountAsync(Guid contentId, int newAttachmentsCount, int attachmentsToRemoveCount, CancellationToken cancellationToken)
        {
            var existingFiles = await _contentFileRepo.GetByContentIdAsync(contentId, cancellationToken);
            var totalFilesAfterUpdate = existingFiles.Count + newAttachmentsCount - attachmentsToRemoveCount;

            if (totalFilesAfterUpdate > ContentConstants.MaxAttachmentsPerContent)
                throw new BadRequestException($"Maximum {ContentConstants.MaxAttachmentsPerContent} attachments allowed per content");
        }

        public async Task DeleteAllAttachmentsAsync(Guid contentId, CancellationToken cancellationToken)
        {
            var attachments = await _contentFileRepo.GetByContentIdAsync(contentId, cancellationToken);
            foreach (var attachment in attachments)
            {
                await _fileService.DeleteAsync(attachment.FileUrl);
                await _contentFileRepo.DeleteAsync(attachment.Id, cancellationToken);
            }
        }
    }
}
