using Application.Common.Interfaces;
using Domain.Constants;

namespace Application.Features.Contents.Commands.Create
{
    public sealed class CreateContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        ISectionRepository _sectionRepo,
        IVideoDurationService _videoDurationService,
        IContentAttachmentService _attachmentService)
        : IRequestHandler<CreateContentCommand, Guid>
    {
        public async Task<Guid> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            if (await _sectionRepo.GetEntityByIdAsync(request.Dto.SectionId, cancellationToken) is null)
                throw new NotFoundException("Section", request.Dto.SectionId);

            // Validate attachments count
            if (request.Dto.Attachments.Count > ContentConstants.MaxAttachmentsPerContent)
                throw new BadRequestException($"Maximum {ContentConstants.MaxAttachmentsPerContent} attachments allowed per content");

            // Upload video file
            var videoFolder = $"{FolderPaths.sectionContentVideos}/{request.Dto.SectionId}";
            var videoUrl = await _fileService.UploadAsync(request.Dto.VideoFile.OpenReadStream(), request.Dto.VideoFile.FileName, videoFolder);

            // Calculate video duration
            var durationInSeconds = await _videoDurationService.GetDurationAsync(request.Dto.VideoFile, cancellationToken);

            var content = request.Dto.ToEntity(videoUrl, durationInSeconds);
            var createdContentId = await _repo.CreateAsync(content, cancellationToken);

            // Upload attachments
            await _attachmentService.UploadAttachmentsAsync(createdContentId, request.Dto.Attachments, cancellationToken);

            return createdContentId;
        }
    }
}
