namespace Application.DTOs.Authentication;

public class GoogleUserDto
{
    public string GoogleId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}