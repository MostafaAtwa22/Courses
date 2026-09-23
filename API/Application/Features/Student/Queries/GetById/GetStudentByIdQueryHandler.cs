using Application.Common.Interfaces.Cache;
using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetById;

public sealed class GetStudentByIdQueryHandler(
    IStudentRepository _studentRepository,
    IAppCache _cache)
    : IRequestHandler<GetStudentByIdQuery, StudentResponseDto?>
{
    public async Task<StudentResponseDto?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.Student(request.Id);
        var cacheOptions = new CacheOptions(
            Expiration: TimeSpan.FromMinutes(1),
            LocalCacheExpiration: TimeSpan.FromSeconds(30));

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async ct => await _studentRepository.GetByIdAsync(request.Id, ct),
            cacheOptions,
            tags: new[] { CacheKeys.Students() },
            cancellationToken);
    }
}
