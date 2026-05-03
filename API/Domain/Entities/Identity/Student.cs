namespace Domain.Entities.Identity
{
    public class Student : BaseEntity
    {
        public int Coins { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
        
        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
