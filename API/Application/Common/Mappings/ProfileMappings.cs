using Application.DTOs.Profile;
using Domain.Entities.Identity;

namespace Application.Common.Mappings
{
    public static class ProfileMappings
    {
        public static void UpdateFromDto(this ApplicationUser user, UpdateProfileDto dto)
        {
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Gender = dto.Gender;
        }
    }
}
