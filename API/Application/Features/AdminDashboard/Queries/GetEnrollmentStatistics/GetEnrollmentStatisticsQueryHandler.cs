using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.AdminDashboard;

namespace Application.Features.AdminDashboard.Queries.GetEnrollmentStatistics
{
    public sealed class GetEnrollmentStatisticsQueryHandler(
        IAdminDashboardRepository _repo,
        IAppCache _cache)
        : IRequestHandler<GetEnrollmentStatisticsQuery, IEnumerable<EnrollmentStatisticsDto>>
    {
        public async Task<IEnumerable<EnrollmentStatisticsDto>> Handle(GetEnrollmentStatisticsQuery request, CancellationToken ct)
        {
            var cacheKey = CacheKeys.EnrollmentStatistics();
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(2),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async cancellationToken => await _repo.GetEnrollmentStatisticsAsync(cancellationToken),
                cacheOptions,
                tags: new[] { CacheKeys.AdminDashboard() },
                ct);
            
            return result ?? Enumerable.Empty<EnrollmentStatisticsDto>();
        }
    }
}
