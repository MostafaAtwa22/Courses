using Domain.Enums;

namespace Application.DTOs.Authentication
{
    public class AuthResponseDto : BaseIdentityResponseDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public string? Provider { get; set; }
    }
}