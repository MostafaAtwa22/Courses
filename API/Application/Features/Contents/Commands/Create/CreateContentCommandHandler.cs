using Domain.Constants;
using TagLib;
using System.IO;

namespace Application.Features.Contents.Commands.Create
{
    public sealed class CreateContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        ISectionRepository _sectionRepo,
        IContentFileRepository _contentFileRepo)
        : IRequestHandler<CreateContentCommand, Guid>
    {
        public async Task<Guid> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            if (await _sectionRepo.GetEntityByIdAsync(request.Dto.SectionId, cancellationToken) is null)
                throw new NotFoundException("Section", request.Dto.SectionId);

            // Validate attachments count (max 5)
            if (request.Dto.Attachments.Count > 5)
                throw new BadRequestException("Maximum 5 attachments allowed per content");

            // Upload video file
            var videoFolder = $"{FolderPaths.sectionContentVideos}/{request.Dto.SectionId}";
            var videoUrl = await _fileService.UploadAsync(request.Dto.VideoFile.OpenReadStream(), request.Dto.VideoFile.FileName, videoFolder);

            // Calculate video duration using TagLibSharp
            double durationInSeconds = 0;
            try
            {
                var tempFilePath = Path.Combine(Path.GetTempPath(), request.Dto.VideoFile.FileName);
                using (var fileStream = System.IO.File.Create(tempFilePath))
                {
                    await request.Dto.VideoFile.CopyToAsync(fileStream, cancellationToken);
                }

                var tagFile = TagLib.File.Create(tempFilePath);
                durationInSeconds = tagFile.Properties.Duration.TotalSeconds;

                System.IO.File.Delete(tempFilePath);
            }
            catch
            {
                // If duration calculation fails, set to 0
                durationInSeconds = 0;
            }

            var content = request.Dto.ToEntity(videoUrl, durationInSeconds);
            var createdContentId = await _repo.CreateAsync(content, cancellationToken);

            // Upload attachments
            var attachmentFolder = $"{FolderPaths.contentAttachments}/{createdContentId}";
            foreach (var attachment in request.Dto.Attachments)
            {
                var attachmentUrl = await _fileService.UploadAsync(attachment.OpenReadStream(), attachment.FileName, attachmentFolder);
                var contentFile = new Domain.Entities.ContentFile
                {
                    Id = Guid.NewGuid(),
                    FileName = attachment.FileName,
                    FileUrl = attachmentUrl,
                    ContentId = createdContentId,
                    CreatedAt = DateTime.UtcNow
                };
                await _contentFileRepo.CreateAsync(contentFile, cancellationToken);
            }

            return createdContentId;
        }
    }
}
