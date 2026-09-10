namespace Application.DTOs.AdminDashboard
{
    public class RoleStatisticsDto
    {
        public int SuperAdminCount { get; set; }
        public double SuperAdminChange { get; set; }
        public int AdminCount { get; set; }
        public double AdminChange { get; set; }
        public int InstructorCount { get; set; }
        public double InstructorChange { get; set; }
        public int StudentCount { get; set; }
        public double StudentChange { get; set; }
    }
}
