using Application.DTOs.Authorization.Permissions;

namespace Application.Features.Permissions.Commands
{
    public sealed record UpdateRolePermissionsCommand(string Id, ICollection<PermissionCheckboxDto> PermissionsDto) : IRequest;

}