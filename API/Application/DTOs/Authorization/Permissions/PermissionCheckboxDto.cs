namespace Application.DTOs.Authorization.Permissions
{
    public class PermissionCheckboxDto
    {
        public string Name { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}