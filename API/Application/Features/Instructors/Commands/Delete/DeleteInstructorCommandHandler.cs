using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;

namespace Application.Features.Instructors.Commands.Delete
{
    public sealed class DeleteInstructorCommandHandler(
        IInstructorRepository _repo,
        IAppCache _cache)
        : IRequestHandler<DeleteInstructorCommand>
    {
        public async Task Handle(DeleteInstructorCommand request, CancellationToken cancellationToken)
        {
            await _repo.DeleteAsync(request.Id, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Instructors(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Instructor(request.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
        }
    }
}
