using Application.Common.Interfaces;
using Application.DTOs.AdminDashboard;

namespace Application.Features.AdminDashboard.Queries.GetEnrollmentStatistics
{
    public sealed class GetEnrollmentStatisticsQueryHandler(IAdminDashboardRepository _repo)
        : IRequestHandler<GetEnrollmentStatisticsQuery, IEnumerable<EnrollmentStatisticsDto>>
    {
        public Task<IEnumerable<EnrollmentStatisticsDto>> Handle(GetEnrollmentStatisticsQuery request, CancellationToken ct)
        {
            return _repo.GetEnrollmentStatisticsAsync(ct);
        }
    }
}
