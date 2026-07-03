namespace Infrastructure.Identity.Authentication.Github;

public class GithubOptions
{
    public const string SectionName = "Authentication:Github";
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
