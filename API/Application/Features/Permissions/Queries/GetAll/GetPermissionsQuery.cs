using Application.DTOs.Authorization.Permissions;

namespace Application.Features.Permissions.Queries.GetAll
{
    public sealed record GetPermissionsQuery(string Id) : IRequest<PermissionRoleDto>;
}
