using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.InstructorDashboard;

namespace Application.Features.InstructorDashboard.Queries.GetInstructorEnrollmentStatistics
{
    public sealed class GetInstructorEnrollmentStatisticsQueryHandler(
        IInstructorDashboardRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetInstructorEnrollmentStatisticsQuery, IEnumerable<InstructorEnrollmentStatisticsDto>>
    {
        public async Task<IEnumerable<InstructorEnrollmentStatisticsDto>> Handle(GetInstructorEnrollmentStatisticsQuery request, CancellationToken ct)
        {
            var cacheKey = CacheKeys.InstructorEnrollmentStatistics(request.InstructorId);
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(2),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetInstructorEnrollmentStatisticsAsync(request.InstructorId, cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.InstructorDashboard() },
                ct);

            return result ?? Enumerable.Empty<InstructorEnrollmentStatisticsDto>();
        }
    }
}
