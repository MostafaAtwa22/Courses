using Application.DTOs.InstructorDashboard;

namespace Application.Features.InstructorDashboard.Queries.GetInstructorStatistics
{
    public sealed record GetInstructorStatisticsQuery(Guid InstructorId) : IRequest<InstructorStatisticsDto>;
}