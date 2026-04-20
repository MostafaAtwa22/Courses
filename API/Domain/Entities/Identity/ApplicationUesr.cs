using Microsoft.AspNetCore.Identity;
using Domain.Enums;

namespace Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? GoogleId { get; set; }
    }
}