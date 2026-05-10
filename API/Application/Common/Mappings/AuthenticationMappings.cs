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
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                NormalizedEmail = dto.Email.ToUpper(),
                NormalizedUserName = dto.UserName.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }
    }
}