namespace Infrastructure.Identity.Authentication.Google;

public class GoogleOptions
{
    public const string SectionName = "Authentication:Google";
    public string ClientId { get; set; } = string.Empty;
}
