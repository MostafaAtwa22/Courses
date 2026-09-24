using Application.DTOs.InstructorDashboard;

namespace Application.Common.Interfaces
{
    public interface IInstructorDashboardRepository
    {
        Task<InstructorStatisticsDto?> GetInstructorStatisticsAsync(Guid instructorId, CancellationToken ct = default);
    }
}