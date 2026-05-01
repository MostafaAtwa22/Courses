using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public string Headline { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
    }
}
