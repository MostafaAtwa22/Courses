namespace Application.DTOs.Authorization;

public class UserRolesManageDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<CheckBoxRoleManageDto> Roles { get; set; } = [];
}
