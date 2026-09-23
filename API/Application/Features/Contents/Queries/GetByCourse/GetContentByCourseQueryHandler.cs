using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetByCourse
{
    public sealed class GetContentByCourseQueryHandler(
        IContentRepository _repo,
        IAppCache _cache) 
        : IRequestHandler<GetContentByCourseQuery, PaginatedResult<ContentResponseDto>>
    {
        public async Task<PaginatedResult<ContentResponseDto>> Handle(GetContentByCourseQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.QueryParams.PageNumber ?? 1;
            var pageSize = request.QueryParams.PageSize ?? 10;
            var cacheKey = CacheKeys.ContentsByCourse(request.CourseId, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetByCourseAsync(request.CourseId, request.QueryParams, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Contents(), CacheKeys.Courses() },
                cancellationToken);
            
            return result ?? new PaginatedResult<ContentResponseDto>([], 0, pageNumber, pageSize);
        }
    }
}
