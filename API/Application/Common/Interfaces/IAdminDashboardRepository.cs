using Application.DTOs.AdminDashboard;

namespace Application.Common.Interfaces
{
    public interface IAdminDashboardRepository
    {
        Task<IEnumerable<EnrollmentStatisticsDto>> GetEnrollmentStatisticsAsync(CancellationToken ct = default!);
    }
}
