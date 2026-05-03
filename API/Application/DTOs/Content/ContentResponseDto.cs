using Domain.Enums;

namespace Application.DTOs.Content
{
    public class ContentResponseDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public ContentType Type { get; set; }
        public string ContentUrl { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsPreview { get; set; }
        public Guid SectionId { get; set; }
    }
}
