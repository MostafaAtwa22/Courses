namespace Domain.Entities
{
    public class ContentProgress : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        
        public Guid ContentId { get; set; }
        public Content Content { get; set; } = null!;
        
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
