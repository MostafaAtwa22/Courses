using Application.Common.Interfaces.Cache;

namespace Application.Features.Sections.Commands.Delete
{
    public sealed class DeleteSectionCommandHandler(
        ISectionRepository _repo,
        IAppCache _cache) : IRequestHandler<DeleteSectionCommand>
    {
        public async Task Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            if (await _repo.GetEntityByIdAsync(request.Id, cancellationToken) is null)
                throw new NotFoundException("Section", request.Id);

            await _repo.DeleteAsync(request.Id, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Sections(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Section(request.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Contents(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
        }
    }
}
