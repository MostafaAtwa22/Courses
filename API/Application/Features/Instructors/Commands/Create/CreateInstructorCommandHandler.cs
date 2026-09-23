using Application.Common.Interfaces.Cache;

namespace Application.Features.Instructors.Commands.Create
{
    public sealed class CreateInstructorCommandHandler(
        IInstructorRepository _repo,
        IFileService _fileService,
        IAppCache _cache) : IRequestHandler<CreateInstructorCommand, Guid>
    {
        public async Task<Guid> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
        {
            var cvUrl = await _fileService.UploadAsync(
                request.Dto.CvUrl.OpenReadStream(),
                request.Dto.CvUrl.FileName,
                FolderPaths.CVs
            );

            var instructor = request.Dto.ToEntity(cvUrl, request.User!.Id);

            var instructorId = await _repo.CreateAsync(instructor, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Instructors(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);

            return instructorId;
        }
    }
}
