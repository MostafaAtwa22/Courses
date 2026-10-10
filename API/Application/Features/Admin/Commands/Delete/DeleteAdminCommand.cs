using Domain.Enums.Identity;

namespace Application.Features.Admin.Commands.Delete;

public sealed record DeleteAdminCommand(Guid Id)
    : IRequest, IRequireAuthorization
{
    public string[] RequiredRoles => [];
    public string[] RequiredPermissions => [PermissionConstants.Build(Module.Admin, CRUD.Delete)];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}