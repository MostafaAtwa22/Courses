using Domain.Enums.Identity;

namespace Application.DTOs.Admin;

public class AdminCommonResponseDto : BaseUserResponseDto
{
    public Role Role { get; set; }
}