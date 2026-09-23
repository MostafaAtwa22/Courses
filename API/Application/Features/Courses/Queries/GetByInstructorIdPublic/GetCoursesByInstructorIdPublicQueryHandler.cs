using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetByInstructorIdPublic
{
    public sealed class GetCoursesByInstructorIdPublicQueryHandler(
        ICourseRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetCoursesByInstructorIdPublicQuery, PaginatedResult<CourseSummaryDto>>
    {
        public async Task<PaginatedResult<CourseSummaryDto>> Handle(GetCoursesByInstructorIdPublicQuery request, CancellationToken ct)
        {
            var pageNumber = request.QueryParams.PageNumber ?? 1;
            var pageSize = request.QueryParams.PageSize ?? 10;
            var cacheKey = CacheKeys.CoursesByInstructorPublic(request.InstructorId, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(15),
                LocalCacheExpiration: TimeSpan.FromMinutes(5));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetPublishedCoursesByInstructorIdAsync(request.InstructorId, request.QueryParams, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.Courses() },
                ct);
            
            return result = new PaginatedResult<CourseSummaryDto>([], 0, pageNumber, pageSize);
        }
    }
}
