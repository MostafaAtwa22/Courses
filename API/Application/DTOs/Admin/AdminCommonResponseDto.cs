using Role = Domain.Enums.Identity.Role;

namespace Application.DTOs.Admin;

public class AdminCommonResponseDto : BaseUserResponseDto
{
    public Role Role { get; set; }
}