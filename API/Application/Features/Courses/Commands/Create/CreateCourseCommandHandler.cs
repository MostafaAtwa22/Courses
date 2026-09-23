using Application.Common.Interfaces.Cache;

namespace Application.Features.Courses.Commands.Create
{
    public sealed class CreateCourseCommandHandler(
        ICourseRepository _repo, 
        IFileService _fileService,
        IAppCache _cache) : IRequestHandler<CreateCourseCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var pictureUrl = await _fileService.UploadAsync(
                request.Dto.PictureUrl.OpenReadStream(),
                request.Dto.PictureUrl.FileName,
                FolderPaths.Courses
            );

            string introVideoUrl = null!;
            if (request.Dto.IntroVideo is not null)
            {
                introVideoUrl = await _fileService.UploadAsync(
                    request.Dto.IntroVideo.OpenReadStream(),
                    request.Dto.IntroVideo.FileName,
                    FolderPaths.sectionContentVideos + $"{request.Dto.Title.Replace(" ", "_")}_Intro_Video"
                );
            }

            var course = request.Dto.ToEntity(pictureUrl, introVideoUrl);

            var courseId = await _repo.CreateAsync(course, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Instructors(), cancellationToken);

            return courseId;
        }
    }
}