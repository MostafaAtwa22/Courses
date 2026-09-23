using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Category;
using MediatR;

namespace Application.Features.Categories.Queries.GetById
{
    public sealed class GetCategoryByIdQueryHandler(
        ICategoryRepository _repo,
        IAppCache _cache) : IRequestHandler<GetCategoryByIdQuery, CategoryResponseDto?>
    {

        public async Task<CategoryResponseDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.Category(request.id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromHours(1),
                LocalCacheExpiration: TimeSpan.FromMinutes(30));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetByIdAsync(request.id, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Categories() },
                cancellationToken);
        }
    }
}