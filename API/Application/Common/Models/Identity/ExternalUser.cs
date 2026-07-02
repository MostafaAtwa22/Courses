using Domain.Enums.Identity;

namespace Application.Common.Models.Identity;

public class ExternalUser
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Picture { get; set; } = string.Empty;
    public ExternalLoginProvider Provider { get; set; }
}
