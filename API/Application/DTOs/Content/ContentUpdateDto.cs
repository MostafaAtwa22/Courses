using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Content
{
    public class ContentUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile? VideoFile { get; set; }
        public List<IFormFile> AttachmentsToAdd { get; set; } = new();
        public List<Guid> AttachmentIdsToRemove { get; set; } = new();
        public int Order { get; set; }
        public bool IsPreview { get; set; }
        public Guid SectionId { get; set; }
    }
}
