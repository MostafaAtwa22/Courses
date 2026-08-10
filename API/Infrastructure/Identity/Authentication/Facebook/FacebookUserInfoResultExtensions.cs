using Application.Common.Extensions;
using Application.Common.Models.Identity;
using Domain.Enums.Identity;

namespace Infrastructure.Identity.Authentication.Facebook;

public static class FacebookUserInfoResultExtensions
{
    public static ExternalUser MapToFacebook(this FacebookValidator.FacebookUserInfoResult facebookUserInfo)
    {
        return new ExternalUser
        {
            Id = facebookUserInfo.Id,
            Email = facebookUserInfo.Email,
            FirstName = facebookUserInfo.FirstName,
            LastName = facebookUserInfo.LastName,
            Gender = GenderMappingExtensions.MapGender(facebookUserInfo.Gender),
            Provider = ExternalLoginProvider.Facebook
        };
    }
}