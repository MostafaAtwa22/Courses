using Application.DTOs.Admin;
using Role = Domain.Enums.Identity.Role;

namespace Application.Features.Admin.Queries.GetAll;

public sealed record GetAllAdminsQuery(AdminQueryParams Params)
    : IRequest<PaginatedResult<AdminResponseDto>>, IRequireAuthorization
{
    public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}