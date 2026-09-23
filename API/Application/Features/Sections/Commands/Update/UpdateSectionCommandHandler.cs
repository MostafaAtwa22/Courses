using Application.Common.Interfaces.Cache;

namespace Application.Features.Sections.Commands.Update
{
    public sealed class UpdateSectionCommandHandler(
        ISectionRepository _repo,
        IAppCache _cache) : IRequestHandler<UpdateSectionCommand>
    {
        public async Task Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Section", request.Id);

            request.Dto.UpdateEntity(section);
            await _repo.UpdateAsync(section, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Sections(), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.Section(request.Id), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Contents(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);
        }
    }
}
