using Application.DTOs.Account;
using Domain.Entities.Identity;
using Application.Common.Interfaces;

namespace Application.Common.Mappings
{
    public static class AccountMappings
    {
        public static UserResponseDto ToUserResponseDto(this ApplicationUser user, IList<string> roles, IUrlProvider urlProvider)
        {
            return new UserResponseDto
            {
                Id = Guid.Parse(user.Id),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                ProfilePicture = urlProvider.GetFullUrl(user.ProfilePictureUrl),
                Gender = user.Gender,
                Is2FAEnable = user.TwoFactorEnabled,
                Roles = [.. roles]
            };
        }
    }
}
