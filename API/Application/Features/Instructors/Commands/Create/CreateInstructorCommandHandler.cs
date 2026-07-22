using Application.Common.Interfaces.Identity;
using Application.Common.Mappings;

namespace Application.Features.Instructors.Commands.Create
{
    public sealed class CreateInstructorCommandHandler(
        IInstructorRepository _repo, 
        IFileService _fileService) : IRequestHandler<CreateInstructorCommand, Guid>
    {
        public async Task<Guid> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
        {
            var cvUrl = await _fileService.UploadAsync(
                request.Dto.CvUrl.OpenReadStream(),
                request.Dto.CvUrl.FileName,
                FolderPaths.CVs
            );

            var instructor = request.Dto.ToEntity(cvUrl, request.User!.Id);

            return await _repo.CreateAsync(instructor, cancellationToken);
        }
    }
}
