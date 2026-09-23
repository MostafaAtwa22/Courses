using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetCurrentInstructor
{
    public sealed class GetCurrentInstructorQueryHandler(
        IInstructorRepository _repo,
        ICurrentUserService _currentUserService,
        IAppCache _cache)
        : IRequestHandler<GetCurrentInstructorQuery, InstructorPrivateResponseDto?>
    {
        public async Task<InstructorPrivateResponseDto?> Handle(
            GetCurrentInstructorQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");

            var cacheKey = CacheKeys.InstructorByUser(userId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(5),
                LocalCacheExpiration: TimeSpan.FromMinutes(2));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetPrivateByUserIdAsync(userId, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Instructors() },
                cancellationToken);
        }
    }
}
