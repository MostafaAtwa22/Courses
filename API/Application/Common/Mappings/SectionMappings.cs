using Application.DTOs.Section;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class SectionMappings
    {
        public static Section ToEntity(this SectionCreateDto dto)
        {
            return new Section
            {
                Title = dto.Title,
                Order = dto.Order,
                CourseId = dto.CourseId
            };
        }

        public static void UpdateEntity(this SectionUpdateDto dto, Section section)
        {
            section.Title = dto.Title;
            section.Order = dto.Order;
        }
    }
}
