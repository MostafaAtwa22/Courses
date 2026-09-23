using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPrivateById
{
    public sealed class GetPrivateInstructorByIdQueryHandler(
        IInstructorRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetPrivateInstructorByIdQuery, InstructorPrivateResponseDto?>
    {
        public async Task<InstructorPrivateResponseDto?> Handle(
            GetPrivateInstructorByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.Instructor(request.Id);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(2),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetPrivateByIdAsync(request.Id, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Instructors() },
                cancellationToken);
        }
    }
}
