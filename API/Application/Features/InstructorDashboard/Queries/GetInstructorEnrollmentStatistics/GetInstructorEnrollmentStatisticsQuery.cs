using Application.DTOs.InstructorDashboard;

namespace Application.Features.InstructorDashboard.Queries.GetInstructorEnrollmentStatistics
{
    public sealed record GetInstructorEnrollmentStatisticsQuery(Guid InstructorId) : IRequest<IEnumerable<InstructorEnrollmentStatisticsDto>>;
}
