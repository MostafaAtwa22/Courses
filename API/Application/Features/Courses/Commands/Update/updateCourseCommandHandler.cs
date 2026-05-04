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

            string? introVideoUrl = null;
            if (request.Dto.IntroVideo is not null)
            {
                // delete the old one
                if (!string.IsNullOrEmpty(course.IntroVideoUrl))
                {
                    await _fileService.DeleteAsync(course.IntroVideoUrl);
                }

                // add the new one
                introVideoUrl = await _fileService.UploadAsync(
                    request.Dto.IntroVideo.OpenReadStream(),
                    request.Dto.IntroVideo.FileName,
                    FolderPaths.sectionContentVideos + $"{request.Dto.Title.Replace(" ", "_")}_Intro_Video"
                );
            }

            request.Dto.UpdateEntity(course, pictureUrl, introVideoUrl);
            
            await _repo.UpdateAsync(course, cancellationToken);
        }
    }
}