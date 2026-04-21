namespace Application.DTOs.Category
{
    public class CategoryResponseDto : BaseResponseDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
    }
}