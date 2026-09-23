using Application.Common.Interfaces.Cache;
using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetAll;

public sealed class GetStudentsQueryHandler(
    IStudentRepository _studentRepository,
    IAppCache _cache)
    : IRequestHandler<GetStudentsQuery, PaginatedResult<StudentResponseDto>>
{
    public async Task<PaginatedResult<StudentResponseDto>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.Params.PageNumber ?? 1;
        var pageSize = request.Params.PageSize ?? 10;
        var searchTerm = request.Params.SearchTerm ?? string.Empty;
        var cacheKey = CacheKeys.Students(searchTerm, pageNumber, pageSize);
        var cacheOptions = new CacheOptions(
            Expiration: TimeSpan.FromMinutes(1),
            LocalCacheExpiration: TimeSpan.FromSeconds(30));

        var result = await _cache.GetOrCreateAsync(
            cacheKey,
            async ct => await _studentRepository.GetAllAsync(request.Params, ct),
            cacheOptions,
            tags: new[] { CacheKeys.Students() },
            cancellationToken);

        return result ?? new PaginatedResult<StudentResponseDto>([], 0, pageNumber, pageSize);
    }
}
