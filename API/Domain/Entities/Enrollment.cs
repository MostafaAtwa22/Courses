using System;
using Domain.Entities.Identity;

namespace Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
