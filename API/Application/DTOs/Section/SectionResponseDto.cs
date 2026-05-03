namespace Application.DTOs.Section
{
    public class SectionResponseDto : BaseResponseDto
    {
        public string Title { get; set; } = string.Empty;

        public int Order { get; set; }
        public int ContentsCount { get; set; }
    }
}