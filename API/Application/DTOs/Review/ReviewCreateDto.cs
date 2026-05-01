namespace Application.DTOs.Review
{
    public class ReviewCreateDto : ReviewBaseDto
    {
        public Guid CourseId { get; set; }
    }
}
