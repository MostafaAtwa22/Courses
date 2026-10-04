using Application.DTOs.Admin;
using Role = Domain.Enums.Identity.Role;

namespace Application.Features.Admin.Queries.GetById;

public sealed record GetAdminByIdQuery(Guid Id)
    : IRequest<AdminResponseDto?>, IRequireAuthorization
{
    public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}