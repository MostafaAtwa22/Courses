using Application.Common.Interfaces;
using Domain.Constants;

namespace Application.Features.Contents.Commands.Update
{
    public sealed class UpdateContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        ISectionRepository _sectionRepo,
        IVideoDurationService _videoDurationService,
        IContentAttachmentService _attachmentService)
        : IRequestHandler<UpdateContentCommand>
    {
        public async Task Handle(UpdateContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Content", request.Id);

            if (await _sectionRepo.GetEntityByIdAsync(request.Dto.SectionId, cancellationToken) is null)
                throw new NotFoundException("Section", request.Dto.SectionId);

            // Validate attachments count
            await _attachmentService.ValidateAttachmentCountAsync(
                request.Id,
                request.Dto.AttachmentsToAdd.Count,
                request.Dto.AttachmentIdsToRemove.Count,
                cancellationToken);

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
                newDurationInSeconds = await _videoDurationService.GetDurationAsync(request.Dto.VideoFile, cancellationToken);
            }

            // Delete specified attachments
            await _attachmentService.DeleteAttachmentsAsync(request.Dto.AttachmentIdsToRemove, cancellationToken);

            // Upload new attachments
            await _attachmentService.UploadAttachmentsAsync(request.Id, request.Dto.AttachmentsToAdd, cancellationToken);

            request.Dto.UpdateEntity(content, newUrl, newDurationInSeconds);
            await _repo.UpdateAsync(content, cancellationToken);
        }
    }
}
