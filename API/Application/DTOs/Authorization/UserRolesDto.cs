namespace Application.DTOs.Authorization;

public class UserRolesDto
{
    public ICollection<CheckBoxRoleManageDto> Roles { get; set; } = [];
}