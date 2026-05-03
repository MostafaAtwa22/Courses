using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Content
{
    public class ContentCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public ContentType Type { get; set; }
        public IFormFile File { get; set; } = default!;
        public int Order { get; set; }
        public bool IsPreview { get; set; }
        public Guid SectionId { get; set; }
    }
}
