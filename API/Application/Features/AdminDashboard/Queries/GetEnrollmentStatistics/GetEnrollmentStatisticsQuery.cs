using Application.DTOs.AdminDashboard;

namespace Application.Features.AdminDashboard.Queries.GetEnrollmentStatistics
{
    public sealed record GetEnrollmentStatisticsQuery() : IRequest<IEnumerable<EnrollmentStatisticsDto>>;
}
