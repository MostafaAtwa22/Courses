using Application.DTOs.Admin;
using Domain.Enums.Identity;

namespace Application.Features.Admin.Queries.GetAll;

public sealed record GetAllAdminsQuery(AdminQueryParams Params)
    : IRequest<PaginatedResult<AdminResponseDto>>, IRequireAuthorization
{
    public string[] RequiredRoles => [];
    public string[] RequiredPermissions => [PermissionConstants.Build(Module.Admin, CRUD.Read)];
    public bool RequireOwnership => false;
    public Guid ResourceId => Guid.Empty;
}