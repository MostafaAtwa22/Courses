using Application.DTOs.Admin;
using Domain.Enums.Identity;

namespace Application.Features.Admin.Queries.GetById;

public sealed record GetAdminByIdQuery(Guid Id)
    : IRequest<AdminResponseDto?>, IRequireAuthorization
{
    public string[] RequiredRoles => [];
    public string[] RequiredPermissions => [PermissionConstants.Build(Module.Admin, CRUD.Read)];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}