namespace Domain.Entities
{
    public class ContentFile : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;

        public string FileUrl { get; set; } = string.Empty;

        public Guid ContentId { get; set; }
        public Content Content { get; set; } = default!;
    }
}
