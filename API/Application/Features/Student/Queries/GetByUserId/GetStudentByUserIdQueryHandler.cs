using Application.Common.Interfaces.Cache;
using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetByUserId;

public sealed class GetStudentByUserIdQueryHandler(
    IStudentRepository _studentRepository,
    IAppCache _cache)
    : IRequestHandler<GetStudentByUserIdQuery, StudentResponseDto?>
{
    public async Task<StudentResponseDto?> Handle(GetStudentByUserIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.StudentByUser(request.UserId);
        var cacheOptions = new CacheOptions(
            Expiration: TimeSpan.FromMinutes(2),
            LocalCacheExpiration: TimeSpan.FromMinutes(1));

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async ct =>
            {
                var student = await _studentRepository.GetByUserIdAsync(request.UserId, ct)
                    ?? throw new NotFoundException("Student", Guid.Parse(request.UserId));

                return await _studentRepository.GetByIdAsync(student.Id, ct);
            },
            cacheOptions,
            tags: new[] { CacheKeys.Students() },
            cancellationToken);
    }
}
