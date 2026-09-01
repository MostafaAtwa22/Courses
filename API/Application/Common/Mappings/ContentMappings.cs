using Application.DTOs.Content;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class ContentMappings
    {
        public static Content ToEntity(this ContentCreateDto dto, string contentUrl, double durationInSeconds)
        {
            return new Content
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                ContentUrl = contentUrl,
                DurationInSeconds = durationInSeconds,
                Order = dto.Order,
                IsPreview = dto.IsPreview,
                SectionId = dto.SectionId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this ContentUpdateDto dto, Content entity, string? newContentUrl = null, double? newDurationInSeconds = null)
        {
            entity.Title = dto.Title;
            
            if (newContentUrl != null)
            {
                entity.ContentUrl = newContentUrl;
            }

            if (newDurationInSeconds != null)
            {
                entity.DurationInSeconds = newDurationInSeconds.Value;
            }

            entity.Order = dto.Order;
            entity.IsPreview = dto.IsPreview;
            entity.SectionId = dto.SectionId;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
