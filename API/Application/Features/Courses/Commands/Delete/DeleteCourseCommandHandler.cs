using Application.Common.Interfaces.Cache;

namespace Application.Features.Courses.Commands.Delete
{
    public sealed class DeleteCourseCommandHandler(
        ICourseRepository _repo,
        IFileService _fileService,
        IAppCache _cache) : IRequestHandler<DeleteCourseCommand>
    {
        public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Course), request.Id);

            await _fileService.DeleteAsync(course.PictureUrl);

            await _repo.DeleteAsync(request.Id, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Course(request.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Sections(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Contents(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Reviews(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Discounts(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Progress(), cancellationToken);
        }
    }
}