using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Mappings;
using MediatR;

namespace Application.Features.Sections.Commands.Create
{
    public sealed class CreateSectionCommandHandler(
        ISectionRepository _repo,
        IAppCache _cache) : IRequestHandler<CreateSectionCommand, Guid>
    {
        public async Task<Guid> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            var section = request.Dto.ToEntity();
            var sectionId = await _repo.CreateAsync(section, cancellationToken);

            // Invalidate related caches
            await _cache.RemoveByTagAsync(CacheKeys.Sections(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Contents(), cancellationToken);
            await _cache.RemoveByTagAsync(CacheKeys.Courses(), cancellationToken);

            return sectionId;
        }
    }
}
