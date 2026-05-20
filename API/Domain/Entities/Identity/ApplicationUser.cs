using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? GoogleId { get; set; }
        
        public Student? StudentProfile { get; set; }
        public Instructor? InstructorProfile { get; set; }
        
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    }
}