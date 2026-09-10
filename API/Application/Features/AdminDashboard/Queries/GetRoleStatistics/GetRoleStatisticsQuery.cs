using Application.DTOs.AdminDashboard;
using MediatR;

namespace Application.Features.AdminDashboard.Queries.GetRoleStatistics
{
    public record GetRoleStatisticsQuery : IRequest<RoleStatisticsDto>;
}
