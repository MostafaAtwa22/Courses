using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Application.DTOs.Admin;
using Domain.Enums.Identity;

namespace Application.Features.Admin.Queries.GetAll;

public sealed record GetAllAdminsQuery(AdminQueryParams Params)
    : IRequest<PaginatedResult<AdminResponseDto>>, IRequireAuthorization
{
    public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}