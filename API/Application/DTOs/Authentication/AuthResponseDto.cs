using Domain.Enums;

namespace Application.DTOs.Authentication
{
    public class AuthResponseDto : BaseIdentityResponseDto
    {
        public string Token { get; set; } = string.Empty;
    }
}