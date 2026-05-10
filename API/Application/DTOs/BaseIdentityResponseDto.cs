namespace Application.DTOs
{
    public class BaseIdentityResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
        public Gender Gender { get; set; }
        public ICollection<string> Roles { get; set; } = [];
    }
}