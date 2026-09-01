namespace Application.DTOs.Authorization;

public class RoleResponseDto : BaseResponseDto
{
    public string Name { get; set; } = string.Empty;
    public int UserCount { get; set; }
}