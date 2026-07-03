namespace Application.DTOs.Authentication;

public class GithubLoginDto
{
    public string Code { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
