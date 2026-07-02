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
            Picture = facebookUserInfo.Picture.Data.Url,
            Provider = ExternalLoginProvider.Facebook
        };
    }
}