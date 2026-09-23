using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Queries.GetById
{
    public sealed class GetSectionByIdQueryHandler(
        ISectionRepository _repo,
        IAppCache _cache) : IRequestHandler<GetSectionByIdQuery, SectionResponseDto?>
    {
        public async Task<SectionResponseDto?> Handle(GetSectionByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.Section(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetByIdAsync(request.Id, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Sections() },
                cancellationToken);
        }
    }
}
