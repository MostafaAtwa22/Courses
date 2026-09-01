using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Content
{
    public class ContentCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile VideoFile { get; set; } = default!;
        public List<IFormFile> Attachments { get; set; } = new();
        public int Order { get; set; }
        public bool IsPreview { get; set; }
        public Guid SectionId { get; set; }
        public Guid CourseId { get; set; }
    }
}
