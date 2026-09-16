using Application.DTOs.AdminDashboard;

namespace Application.Features.AdminDashboard.Queries.GetRoleStatistics
{
    public sealed class GetRoleStatisticsQueryHandler(UserManager<ApplicationUser> _userManager)
        : IRequestHandler<GetRoleStatisticsQuery, RoleStatisticsDto>
    {
        public async Task<RoleStatisticsDto> Handle(GetRoleStatisticsQuery request, CancellationToken cancellationToken)
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var instructors = await _userManager.GetUsersInRoleAsync("Instructor");
            var students = await _userManager.GetUsersInRoleAsync("Student");

            return AdminDashboardMappings.ToRoleStatisticsDto(
                superAdmins.Count,
                admins.Count,
                instructors.Count,
                students.Count);
        }
    }
}
