using Domain.Enums;

namespace Application.Features.Contents.Commands.Create
{
    public sealed class CreateContentCommandHandler(
        IContentRepository _repo,
        IFileService _fileService,
        ISectionRepository _sectionRepo)
        : IRequestHandler<CreateContentCommand, Guid>
    {
        public async Task<Guid> Handle(CreateContentCommand request, CancellationToken cancellationToken)
        {
            if (await _sectionRepo.GetEntityByIdAsync(request.Dto.SectionId, cancellationToken) is null)
                throw new NotFoundException("Section", request.Dto.SectionId);

            var folder = request.Dto.Type == ContentType.Video 
                ? $"{FolderPaths.sectionContentVideos}" 
                : FolderPaths.sectionContentFiles
                + $"{request.Dto.SectionId}";
            
            var url = await _fileService.UploadAsync(request.Dto.File.OpenReadStream(), request.Dto.File.FileName, folder);

            var content = request.Dto.ToEntity(url);

            return await _repo.CreateAsync(content, cancellationToken);
        }
    }
}
