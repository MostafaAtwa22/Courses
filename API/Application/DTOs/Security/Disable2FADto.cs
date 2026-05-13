namespace Application.DTOs.Security
{
    public class Disable2FADto
    {
        public string Password { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
    }
}