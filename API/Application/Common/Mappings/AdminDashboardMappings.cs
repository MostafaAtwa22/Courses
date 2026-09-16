using Application.DTOs.AdminDashboard;

namespace Application.Common.Mappings
{
    public static class AdminDashboardMappings
    {
        public static RoleStatisticsDto ToRoleStatisticsDto(
            int superAdminCount,
            int adminCount,
            int instructorCount,
            int studentCount)
        {
            return new RoleStatisticsDto
            {
                SuperAdminCount = superAdminCount,
                SuperAdminChange = 0, 
                AdminCount = adminCount,
                AdminChange = 0, 
                InstructorCount = instructorCount,
                InstructorChange = 0, 
                StudentCount = studentCount,
                StudentChange = 0 
            };
        }
    }
}
