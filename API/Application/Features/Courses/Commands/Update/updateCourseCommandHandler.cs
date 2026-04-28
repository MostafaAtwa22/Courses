namespace Application.Features.Courses.Commands.Update
{
    public sealed class UpdateCourseCommandHandler(
        ICourseRepository _repo,
        IFileService _fileService) : IRequestHandler<UpdateCourseCommand>
    {
        public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Course), request.Id);
            
            string? pictureUrl = null;

            if (request.Dto.PictureUrl is not null)
            {
                // delete the old one
                await _fileService.DeleteAsync(course.PictureUrl);

                // add the new one
                pictureUrl = await _fileService.UploadAsync(
                    request.Dto.PictureUrl.OpenReadStream(),
                    request.Dto.PictureUrl.FileName,
                    FolderPaths.Courses
                );
            }

            request.Dto.UpdateEntity(course, pictureUrl);
            
            await _repo.UpdateAsync(course, cancellationToken);
        }
    }
}