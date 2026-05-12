namespace Application.DTOs.Profile
{
    public class ChangePasswordDto : SetPasswordDto
    {
        public string OldPassword { get; set; } = string.Empty;
    }
}