using Application.DTOs.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Application.Common.Mappings
{
    public static class AuthorizationMappings
    {
        public static RoleResponseDto ToRoleResponseDto(this IdentityRole role, int userCount)
        {
            return new RoleResponseDto
            {
                Id = Guid.Parse(role.Id),
                Name = role.Name ?? string.Empty,
                UserCount = userCount
            };
        }
    }
}
