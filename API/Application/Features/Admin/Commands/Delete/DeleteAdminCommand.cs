using Role = Domain.Enums.Identity.Role;

namespace Application.Features.Admin.Commands.Delete;

public sealed record DeleteAdminCommand(Guid Id)
    : IRequest, IRequireAuthorization
{
    public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}