using Application.DTOs.Category;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class CategoryMappings
    {
        public static Category ToEntity(this CategoryCreateDto dto)
        {
            return new Category
            {
                Name = dto.Name,
                Slug = dto.Slug
            };
        }

        public static void UpdateEntity(this CategoryUpdateDto dto, Category category)
        {
            category.Name = dto.Name;
            category.Slug = dto.Slug;
        }
    }
}
