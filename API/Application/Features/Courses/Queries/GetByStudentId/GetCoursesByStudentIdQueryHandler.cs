using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Course;

namespace Application.Features.Courses.Queries.GetByStudentId
{
    public sealed class GetCoursesByStudentIdQueryHandler(
        ICourseRepository _repo, 
        IStudentRepository _studentRepo,
        ICurrentUserService _currentUserService,
        IAppCache _cache)
        : IRequestHandler<GetCoursesByStudentIdQuery, PaginatedResult<CourseSummaryDto>>
    {
        public async Task<PaginatedResult<CourseSummaryDto>> Handle(GetCoursesByStudentIdQuery request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");
            
            var studentId = await _studentRepo.GetStudentIdByUserIdAsync(userId, ct)
                ?? throw new NotFoundException("Student", Guid.Parse(userId));

            var pageNumber = request.QueryParams.PageNumber ?? 1;
            var pageSize = request.QueryParams.PageSize ?? 10;
            var cacheKey = CacheKeys.CoursesByStudent(userId, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetCoursesByStudentIdAsync(studentId, request.QueryParams, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.Courses() },
                ct);

            return result ?? new PaginatedResult<CourseSummaryDto>([], 0, pageNumber, pageSize);
        }
    }
}
