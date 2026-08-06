namespace Application.DTOs.Progress
{
    public class CourseProgressDto
    {
        public Guid CourseId { get; set; }
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }
        public int PercentComplete { get; set; }
        public List<Guid> CompletedContentIds { get; set; } = new();
    }
}
