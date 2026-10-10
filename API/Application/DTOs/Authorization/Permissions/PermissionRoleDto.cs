namespace Application.DTOs.Authorization.Permissions
{
    public class PermissionRoleDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public ICollection<PermissionCheckboxDto> Permissions { get; set; } = [];
    }
}