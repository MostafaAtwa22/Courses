namespace Application.Common.Mappings
{
    public static class CourseMappings
    {
        public static Course ToEntity(this CourseCreateDto dto, string pictureUrl)
        {
            return new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                PictureUrl = pictureUrl,
                Status = dto.Status,
                Cost = dto.Cost,
                CategoryId = dto.CategoryId
            };
        }

        public static void UpdateEntity(this CourseUpdateDto dto, Course course, string? pictureUrl = null)
        {
            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Status = dto.Status;
            course.Cost = dto.Cost;
            course.CategoryId = dto.CategoryId;
            if (pictureUrl is not null)
                course.PictureUrl = pictureUrl;
        }
    }
}