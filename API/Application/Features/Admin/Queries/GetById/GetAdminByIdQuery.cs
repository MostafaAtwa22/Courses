using Application.Common.Interfaces.Identity;
using Application.DTOs.Admin;
using Domain.Enums.Identity;

namespace Application.Features.Admin.Queries.GetById;

public sealed record GetAdminByIdQuery(Guid Id)
    : IRequest<AdminResponseDto?>, IRequireAuthorization
{
    public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}