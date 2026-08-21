using Domain.Constants;
using TagLib;
using System.IO;

namespace Application.Features.Contents.Commands.Update
{
    public sealed class UpdateContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        ISectionRepository _sectionRepo,
        IContentFileRepository _contentFileRepo)
        : IRequestHandler<UpdateContentCommand>
    {
        public async Task Handle(UpdateContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Content", request.Id);

            if (await _sectionRepo.GetEntityByIdAsync(request.Dto.SectionId, cancellationToken) is null)
                throw new NotFoundException("Section", request.Dto.SectionId);

            // Validate attachments count (max 5 including existing)
            var existingFiles = await _contentFileRepo.GetByContentIdAsync(request.Id, cancellationToken);
            var totalFilesAfterUpdate = existingFiles.Count + request.Dto.AttachmentsToAdd.Count - request.Dto.AttachmentIdsToRemove.Count;
            if (totalFilesAfterUpdate > 5)
                throw new BadRequestException("Maximum 5 attachments allowed per content");

            string? newUrl = null;
            double? newDurationInSeconds = null;

            // Handle video file replacement
            if (request.Dto.VideoFile is not null)
            {
                // Delete old video file
                await _fileService.DeleteAsync(content.ContentUrl);

                // Upload new video file
                var videoFolder = $"{FolderPaths.sectionContentVideos}/{request.Dto.SectionId}";
                newUrl = await _fileService.UploadAsync(request.Dto.VideoFile.OpenReadStream(), request.Dto.VideoFile.FileName, videoFolder);

                // Calculate new video duration
                try
                {
                    var tempFilePath = Path.Combine(Path.GetTempPath(), request.Dto.VideoFile.FileName);
                    using (var fileStream = System.IO.File.Create(tempFilePath))
                    {
                        await request.Dto.VideoFile.CopyToAsync(fileStream, cancellationToken);
                    }

                    var tagFile = TagLib.File.Create(tempFilePath);
                    newDurationInSeconds = tagFile.Properties.Duration.TotalSeconds;

                    System.IO.File.Delete(tempFilePath);
                }
                catch
                {
                    newDurationInSeconds = 0;
                }
            }

            // Delete specified attachments
            foreach (var attachmentId in request.Dto.AttachmentIdsToRemove)
            {
                var attachment = await _contentFileRepo.GetByIdAsync(attachmentId, cancellationToken);
                if (attachment is not null)
                {
                    await _fileService.DeleteAsync(attachment.FileUrl);
                    await _contentFileRepo.DeleteAsync(attachmentId, cancellationToken);
                }
            }

            // Upload new attachments
            var attachmentFolder = $"{FolderPaths.contentAttachments}/{request.Id}";
            foreach (var attachment in request.Dto.AttachmentsToAdd)
            {
                var attachmentUrl = await _fileService.UploadAsync(attachment.OpenReadStream(), attachment.FileName, attachmentFolder);
                var contentFile = new Domain.Entities.ContentFile
                {
                    Id = Guid.NewGuid(),
                    FileName = attachment.FileName,
                    FileUrl = attachmentUrl,
                    ContentId = request.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await _contentFileRepo.CreateAsync(contentFile, cancellationToken);
            }

            request.Dto.UpdateEntity(content, newUrl, newDurationInSeconds);
            await _repo.UpdateAsync(content, cancellationToken);
        }
    }
}
