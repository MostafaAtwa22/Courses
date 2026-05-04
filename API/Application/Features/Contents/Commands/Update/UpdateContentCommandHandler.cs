using Domain.Enums;

namespace Application.Features.Contents.Commands.Update
{
    public sealed class UpdateContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        ISectionRepository _sectionRepo)
        : IRequestHandler<UpdateContentCommand>
    {
        public async Task Handle(UpdateContentCommand request, CancellationToken cancellationToken)
        {
            var content = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Content", request.Id);

            if (await _sectionRepo.GetEntityByIdAsync(request.Dto.SectionId, cancellationToken) is null)   
                throw new NotFoundException("Section", request.Dto.SectionId);

            string? newUrl = null;
            if (request.Dto.File is not null)
            {
                // Delete old file
                await _fileService.DeleteAsync(content.ContentUrl);

                // Upload new file
                var folder = request.Dto.Type == ContentType.Video 
                    ? $"{FolderPaths.sectionContentVideos}" 
                    : FolderPaths.sectionContentFiles
                    + $"{request.Dto.SectionId}";
                
                newUrl = await _fileService.UploadAsync(request.Dto.File.OpenReadStream(), request.Dto.File.FileName, folder);
            }

            request.Dto.UpdateEntity(content, newUrl);
            await _repo.UpdateAsync(content, cancellationToken);
        }
    }
}
