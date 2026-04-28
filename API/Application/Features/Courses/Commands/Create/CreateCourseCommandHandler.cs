namespace Application.Features.Courses.Commands.Create
{
    public sealed class CreateCourseCommandHandler(
        ICourseRepository _repo, 
        IFileService _fileService) : IRequestHandler<CreateCourseCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var pictureUrl = await _fileService.UploadAsync(
                request.Dto.PictureUrl.OpenReadStream(),
                request.Dto.PictureUrl.FileName,
                FolderPaths.Courses
            );

            var course = request.Dto.ToEntity(pictureUrl);

            return await _repo.CreateAsync(course, cancellationToken);
        }
    }
}