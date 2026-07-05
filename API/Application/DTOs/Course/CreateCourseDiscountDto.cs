using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Course
{
    public class CreateCourseDiscountDto
    {
        public decimal Percentage { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }
    }
}
