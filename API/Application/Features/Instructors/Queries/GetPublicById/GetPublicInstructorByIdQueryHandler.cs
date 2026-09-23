using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPublicById
{
    public sealed class GetPublicInstructorByIdQueryHandler(
        IInstructorRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetPublicInstructorByIdQuery, InstructorPublicResponseDto?>
    {
        public async Task<InstructorPublicResponseDto?> Handle(
            GetPublicInstructorByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.InstructorPublic(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(30),
                LocalCacheExpiration: TimeSpan.FromMinutes(10));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetPublicByIdAsync(request.Id, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Instructors() },
                cancellationToken);
        }
    }
}
