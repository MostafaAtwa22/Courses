using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPublicByCourseId
{
    public sealed class GetPublicInstructorByCourseIdQueryHandler(
        IInstructorRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetPublicInstructorByCourseIdQuery, InstructorPublicResponseDto?>
    {
        public async Task<InstructorPublicResponseDto?> Handle(
            GetPublicInstructorByCourseIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.InstructorByCourse(request.CourseId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(30),
                LocalCacheExpiration: TimeSpan.FromMinutes(10));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetPublicByCourseIdAsync(request.CourseId, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Instructors(), CacheKeys.Courses() },
                cancellationToken);
        }
    }
}
