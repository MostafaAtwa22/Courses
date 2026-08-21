namespace Application.DTOs.Content
{
    public class ContentFileResponseDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }

    public class ContentResponseDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public double DurationInSeconds { get; set; }
        public int Order { get; set; }
        public bool IsPreview { get; set; }
        public Guid SectionId { get; set; }
        public List<ContentFileResponseDto> Files { get; set; } = new();
    }
}
