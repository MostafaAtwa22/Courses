using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetAll
{
    public sealed class GetAllInstructorsQueryHandler(
        IInstructorRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetAllInstructorsQuery, PaginatedResult<InstructorPrivateResponseDto>>
    {
        public async Task<PaginatedResult<InstructorPrivateResponseDto>> Handle(
            GetAllInstructorsQuery request, 
            CancellationToken cancellationToken)
        {
            var pageNumber = request.Params.PageNumber ?? 1;
            var pageSize = request.Params.PageSize ?? 10;
            var searchTerm = request.Params.SearchTerm ?? string.Empty;
            var cacheKey = CacheKeys.Instructors(searchTerm, pageNumber, pageSize);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(2),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct => await _repo.GetAllAsync(request.Params, ct),
                cacheOptions,
                tags: new[] { CacheKeys.Instructors() },
                cancellationToken);

            return result ?? new PaginatedResult<InstructorPrivateResponseDto>([], 0, pageNumber, pageSize);
        }
    }
}
