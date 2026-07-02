namespace Infrastructure.Identity.Authentication.Facebook;

public class FacebookOptions
{
    public const string SectionName = "Authentication:Facebook";
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
}
