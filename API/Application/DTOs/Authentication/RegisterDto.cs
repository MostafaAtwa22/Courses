using Domain.Enums;
using Domain.Enums.Identity;

namespace Application.DTOs.Authentication
{
    public class RegisterDto : LoginDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string ConfirmPassword { get; set; } = string.Empty;
        public Role Role { get; set; }
    }
}