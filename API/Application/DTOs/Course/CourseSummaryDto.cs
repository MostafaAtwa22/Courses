namespace Application.DTOs.Course
{
    public class CourseSummaryDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public int Cost { get; set; }
        public int TotalReviews { get; set; }
        public decimal AverageRate { get; set; }
        public string Category { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
    }
}
