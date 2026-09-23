using Application.Common.Interfaces.Cache;
using Application.DTOs.Category;

namespace Application.Features.Categories.Queries.GetAll
{
    public sealed class GetCategoriesQueryHandler (
        ICategoryRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryResponseDto>>
    {

        public async Task<PaginatedResult<CategoryResponseDto>> 
            Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.Params.PageNumber ?? 1;
            var pageSize = request.Params.PageSize ?? 10;
            var searchTerm = request.Params.SearchTerm ?? string.Empty;
            var cacheKey = CacheKeys.Categories(searchTerm, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromHours(1),
                LocalCacheExpiration: TimeSpan.FromMinutes(30));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetAllAsync(request.Params, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Categories() },
                cancellationToken);
            
            return result ?? new PaginatedResult<CategoryResponseDto>([] ,0, pageNumber, pageSize);
        }
    }
}