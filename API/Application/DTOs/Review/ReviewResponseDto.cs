namespace Application.DTOs.Review
{
    public class ReviewResponseDto : BaseResponseDto
    {
        public string Headline { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentProfilePicture { get; set; } = string.Empty;
    }
}
