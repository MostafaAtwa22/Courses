using Application.Common.Interfaces.Cache;
using Application.DTOs.AdminDashboard;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.AdminDashboard.Queries.GetRoleStatistics
{
    public sealed class GetRoleStatisticsQueryHandler(
        UserManager<ApplicationUser> _userManager,
        IAppCache _cache)
        : IRequestHandler<GetRoleStatisticsQuery, RoleStatisticsDto>
    {
        public async Task<RoleStatisticsDto> Handle(GetRoleStatisticsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.RoleStatistics();
            var cacheOptions = new CacheOptions(
                Expiration: TimeSpan.FromMinutes(2),
                LocalCacheExpiration: TimeSpan.FromMinutes(1));

            var result = await _cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
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
                },
                cacheOptions,
                tags: new[] { CacheKeys.AdminDashboard() },
                cancellationToken);

            return result ?? new RoleStatisticsDto();
        }
    }
}
