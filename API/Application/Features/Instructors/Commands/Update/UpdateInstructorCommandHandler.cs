using Application.Common.Interfaces.Identity;
using Application.Common.Mappings;
using Domain.Entities.Identity;

namespace Application.Features.Instructors.Commands.Update
{
    public sealed class UpdateInstructorCommandHandler(
        IInstructorRepository _repo,
        IFileService _fileService) : IRequestHandler<UpdateInstructorCommand>
    {
        public async Task Handle(UpdateInstructorCommand request, CancellationToken cancellationToken)
        {
            var instructor = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Instructor), request.Id);

            string? cvUrl = null;
            if (request.Dto.CvUrl is not null)
            {
                // Delete the old CV before uploading the new one
                if (!string.IsNullOrEmpty(instructor.CvUrl))
                    await _fileService.DeleteAsync(instructor.CvUrl);

                cvUrl = await _fileService.UploadAsync(
                    request.Dto.CvUrl.OpenReadStream(),
                    request.Dto.CvUrl.FileName,
                    FolderPaths.CVs
                );
            }

            request.Dto.UpdateEntity(instructor, cvUrl);

            await _repo.UpdateAsync(instructor, cancellationToken);
        }
    }
}
