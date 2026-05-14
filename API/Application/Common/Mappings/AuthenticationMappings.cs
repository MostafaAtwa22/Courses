using Application.DTOs.Account;
using Application.DTOs.Authentication;
using Domain.Entities.Identity;

namespace Application.Common.Mappings
{
    public static class AuthenticationMappings
    {
        public static ApplicationUser ToApplicationUser(this RegisterDto dto)
        {
            return new ApplicationUser
            {
                UserName           = dto.UserName,
                Email              = dto.Email,
                FirstName          = dto.FirstName,
                LastName           = dto.LastName,
                Gender             = dto.Gender,
                NormalizedEmail    = dto.Email.ToUpper(),
                NormalizedUserName = dto.UserName.ToUpper(),
                SecurityStamp      = Guid.NewGuid().ToString()
            };
        }

        public static AuthResponseDto ToAuthResponseDto(this ApplicationUser user, IList<string> roles)
        {
            return new AuthResponseDto
            {
                Id             = user.Id,
                UserName       = user.UserName          ?? string.Empty,
                Email          = user.Email             ?? string.Empty,
                FirstName      = user.FirstName         ?? string.Empty,
                LastName       = user.LastName          ?? string.Empty,
                PhoneNumber    = user.PhoneNumber       ?? string.Empty,
                Gender         = user.Gender,
                ProfilePicture = user.ProfilePictureUrl ?? string.Empty,
                Is2FAEnable    = user.TwoFactorEnabled,
                Roles          = [..roles]
            };
        }
    }
}