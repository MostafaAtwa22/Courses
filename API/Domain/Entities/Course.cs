using Domain.Enums;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.InProgress;
        public int Cost { get; set; }
        public int StudentCount { get; set; } 
        public int TotalReviews { get; set; } 
        public decimal AverageRate { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = default!;

        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<Enrollment> Enrollments { get; set; } = [];
    }
}