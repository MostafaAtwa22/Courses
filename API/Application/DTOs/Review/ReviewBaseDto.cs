namespace Application.DTOs.Review
{
    public class ReviewBaseDto
    {
        public string Headline { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public decimal Rating { get; set; }
    }
}
