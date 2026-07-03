using Application.Common.Models.Identity;
using Domain.Entities.Identity;
using Domain.Enums.Identity;
using Google.Apis.Auth;

namespace Application.Common.Extensions;

public static class ExternalUserExtensions
{
    public static ExternalUser MapToGoogle(this GoogleJsonWebSignature.Payload payload)
    {
        return new ExternalUser
        {
            Id = payload.Subject,
            Email = payload.Email,
            FirstName = payload.GivenName ?? string.Empty,
            LastName = payload.FamilyName ?? string.Empty,
            Provider = ExternalLoginProvider.Google
        };
    }

    public static ApplicationUser MapToApplicationUser(this ExternalUser externalUser)
    {
        return new ApplicationUser
        {
            Email = externalUser.Email,
            UserName = externalUser.Email,
            FirstName = externalUser.FirstName,
            LastName = externalUser.LastName,
            EmailConfirmed = true
        };
    }
}
