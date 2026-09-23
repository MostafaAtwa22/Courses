using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Section;
using MediatR;

namespace Application.Features.Sections.Queries.GetByCourseId
{
    public sealed class GetSectionsByCourseIdQueryHandler(
        ISectionRepository _repo,
        IAppCache _cache) : IRequestHandler<GetSectionsByCourseIdQuery, PaginatedResult<SectionResponseDto>>
    {
        public async Task<PaginatedResult<SectionResponseDto>> Handle(GetSectionsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            var pageNumber = request.QueryParams.PageNumber ?? 1;
            var pageSize = request.QueryParams.PageSize ?? 10;
            var cacheKey = CacheKeys.SectionsByCourse(request.CourseId, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetByCourseIdAsync(request.CourseId, request.QueryParams, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Sections(), CacheKeys.Courses() },
                cancellationToken);

            return result ?? new PaginatedResult<SectionResponseDto>([], 0, pageNumber, pageSize);
        }
    }
}
