using Application.DTOs.Review;

namespace Application.Common.Mappings
{
    public static class ReviewMappings
    {
        public static Review ToEntity(this ReviewCreateDto dto)
        {
            return new Review
            {
                Headline = dto.Headline,
                Comment = dto.Comment,
                Rating = dto.Rating,
                CourseId = dto.CourseId
            };
        }

        public static void UpdateEntity(this ReviewUpdateDto dto, Review review)
        {
            review.Headline = dto.Headline;
            review.Comment = dto.Comment;
            review.Rating = dto.Rating;
            review.UpdatedAt = DateTime.UtcNow;
        }
    }
}
