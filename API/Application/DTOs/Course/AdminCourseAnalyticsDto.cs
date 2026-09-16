namespace Application.DTOs.Course
{
    public class AdminCourseAnalyticsDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public int EnrolledStudents { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal AverageRating { get; set; }
        public string Status { get; set; } = string.Empty;
        public int SectionsCount { get; set; }
    }
}
