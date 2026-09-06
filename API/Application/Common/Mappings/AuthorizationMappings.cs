using Application.DTOs.Authorization;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Mappings
{
    public static class AuthorizationMappings
    {
        public static RolesResponseDto ToRoleResponseDto(this IdentityRole role, int userCount)
        {
            return new RolesResponseDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                UserCount = userCount
            };
        }

        public static UserRolesResponseDto ToUserRolesManageDto(this ApplicationUser user, IReadOnlyCollection<CheckBoxRoleManageDto> roles)
        {
            return new UserRolesResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList()
            };
        }
    }
}
