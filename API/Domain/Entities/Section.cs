namespace Domain.Entities
{
    public class Section : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public int Order { get; set; }

        public Guid CourseId { get; set; } 
        public Course Course { get; set; } = default!;

        public ICollection<Content> Contents { get; set; } = [];
    }
}