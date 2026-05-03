using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Domain.Constants;
using Domain.Enums;
using MediatR;

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
            var section = await _sectionRepo.GetEntityByIdAsync(request.Dto.SectionId, cancellationToken);
            if (section == null)
            {
                throw new NotFoundException("Section", request.Dto.SectionId);
            }

            var folder = FolderPaths.sectionContent;
            var url = await _fileService.UploadAsync(request.Dto.File.OpenReadStream(), request.Dto.File.FileName, folder);

            var content = request.Dto.ToEntity(url);

            return await _repo.CreateAsync(content, cancellationToken);
        }
    }
}
