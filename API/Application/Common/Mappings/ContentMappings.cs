using Application.DTOs.Content;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class ContentMappings
    {
        public static Content ToEntity(this ContentCreateDto dto, string contentUrl)
        {
            return new Content
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Type = dto.Type,
                ContentUrl = contentUrl,
                Order = dto.Order,
                IsPreview = dto.IsPreview,
                SectionId = dto.SectionId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateEntity(this ContentUpdateDto dto, Content entity, string? newContentUrl = null)
        {
            entity.Title = dto.Title;
            entity.Type = dto.Type;
            
            if (newContentUrl != null)
            {
                entity.ContentUrl = newContentUrl;
            }

            entity.Order = dto.Order;
            entity.IsPreview = dto.IsPreview;
            entity.SectionId = dto.SectionId;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
