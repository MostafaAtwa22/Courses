using Application.DTOs.Account;
using Domain.Entities.Identity;

namespace Application.Common.Mappings
{
    public static class AccountMappings
    {
        public static UserResponseDto ToUserResponseDto(this ApplicationUser user, IList<string> roles)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                ProfilePicture = user.ProfilePictureUrl,
                Gender = user.Gender,
                Roles = [.. roles]
            };
        }
    }
}
