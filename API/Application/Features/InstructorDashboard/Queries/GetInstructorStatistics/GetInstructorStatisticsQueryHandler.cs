using Application.DTOs.InstructorDashboard;

namespace Application.Features.InstructorDashboard.Queries.GetInstructorStatistics
{
    public sealed class GetInstructorStatisticsQueryHandler(
        IInstructorDashboardRepository _repo)
        : IRequestHandler<GetInstructorStatisticsQuery, InstructorStatisticsDto>
    {
        public async Task<InstructorStatisticsDto> Handle(GetInstructorStatisticsQuery request, CancellationToken ct)
        {
            return await _repo.GetInstructorStatisticsAsync(request.InstructorId, ct) ?? new InstructorStatisticsDto();
        }
    }
}