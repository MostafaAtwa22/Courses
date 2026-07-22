namespace Application.DTOs
{
    public class BaseIdentityResponseDto : BaseUserResponseDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public bool Is2FAEnable { get; set; }
        public ICollection<string> Roles { get; set; } = [];
    }
}