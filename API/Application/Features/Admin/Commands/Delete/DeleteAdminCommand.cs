using Application.Common.Interfaces.Identity;
using Domain.Enums.Identity;

namespace Application.Features.Admin.Commands.Delete;

public sealed record DeleteAdminCommand(Guid Id)
    : IRequest, IRequireAuthorization
{
    public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}