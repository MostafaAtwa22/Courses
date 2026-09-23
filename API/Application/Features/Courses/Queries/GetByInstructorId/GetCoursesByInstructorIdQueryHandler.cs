using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetByInstructorId
{
    public sealed class GetCoursesByInstructorIdQueryHandler(
        ICourseRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetCoursesByInstructorIdQuery, PaginatedResult<CourseSummaryDto>>
    {
        public async Task<PaginatedResult<CourseSummaryDto>> Handle(GetCoursesByInstructorIdQuery request, CancellationToken ct)
        {
            var pageNumber = request.QueryParams.PageNumber ?? 1;
            var pageSize = request.QueryParams.PageSize ?? 10;
            var cacheKey = CacheKeys.CoursesByInstructor(request.InstructorId, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetCoursesByInstructorIdAsync(request.InstructorId, request.QueryParams, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.Courses() },
                ct);
            
            return result ?? new PaginatedResult<CourseSummaryDto>([], 0, pageNumber, pageSize);
        }
    }
}
