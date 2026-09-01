namespace Domain.Entities
{
    public class Content : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string ContentUrl { get; set; } = string.Empty;

        public double DurationInSeconds { get; set; }

        public int Order { get; set; }

        public bool IsPreview { get; set; }

        public Guid SectionId { get; set; }
        public Section Section { get; set; } = default!;

        public ICollection<ContentFile> Files { get; set; } = new List<ContentFile>();

        public ICollection<ContentProgress> Progress { get; set; } = [];
    }
}