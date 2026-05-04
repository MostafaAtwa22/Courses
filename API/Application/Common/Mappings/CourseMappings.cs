namespace Application.Common.Mappings
{
    public static class CourseMappings
    {
        public static Course ToEntity(this CourseCreateDto dto, string pictureUrl, string introVideoUrl)
        {
            return new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                PictureUrl = pictureUrl,
                Status = dto.Status,
                Cost = dto.Cost,
                CategoryId = dto.CategoryId,
                InstructorId = dto.InstructorId,
                Language = dto.Language,
                WhatYouWillLearn = dto.WhatYouWillLearn.ToList(),
                Requirements = dto.Requirements.ToList(),
                IntroVideoUrl = introVideoUrl
            };
        }

        public static void UpdateEntity(this CourseUpdateDto dto, Course course, string? pictureUrl = null, string? introVideoUrl = null)
        {
            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Status = dto.Status;
            course.Cost = dto.Cost;
            course.CategoryId = dto.CategoryId;
            course.InstructorId = dto.InstructorId;
            course.Language = dto.Language;
            course.WhatYouWillLearn = dto.WhatYouWillLearn.ToList();
            course.Requirements = dto.Requirements.ToList();
            
            if (pictureUrl is not null)
                course.PictureUrl = pictureUrl;
                
            if (introVideoUrl is not null)
                course.IntroVideoUrl = introVideoUrl;
        }
    }
}