namespace Application.DTOs.Account
{
    public class UserResponseDto : BaseUserResponseDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public ICollection<string> Roles { get; set; } = [];
    }
}