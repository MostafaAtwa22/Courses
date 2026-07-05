using Application.DTOs.Course;
using Domain.Entities;

namespace Application.Common.Mappings
{
    public static class DiscountMappings
    {
        public static CourseDiscount ToEntity(this CreateCourseDiscountDto dto, Guid courseId)
        {
            return new CourseDiscount
            {
                Percentage = dto.Percentage,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                IsActive = true,
                CourseId = courseId
            };
        }

        public static void UpdateEntity(this UpdateCourseDiscountDto dto, CourseDiscount discount)
        {
            discount.Percentage = dto.Percentage;
            discount.StartTime = dto.StartTime;
            discount.EndTime = dto.EndTime;
            discount.IsActive = dto.IsActive;
        }
    }
}
