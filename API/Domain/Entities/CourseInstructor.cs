using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class CourseInstructor : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
    }
}
