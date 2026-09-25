namespace Application.DTOs.InstructorDashboard
{
    public class InstructorStatisticsDto
    {
        public int EnrolledStudents { get; set; }
        public decimal EnrolledStudentsChange { get; set; }
        public decimal MonthlyEarnings { get; set; }
        public decimal MonthlyEarningsChange { get; set; }
        public decimal InstructorRating { get; set; }
        public decimal InstructorRatingChange { get; set; }
        public int CoursesCreated { get; set; }
        public decimal CoursesCreatedChange { get; set; }
    }
}