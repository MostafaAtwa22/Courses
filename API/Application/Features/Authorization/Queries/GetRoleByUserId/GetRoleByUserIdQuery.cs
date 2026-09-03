using Application.DTOs.Authorization;

namespace Application.Features.Authorization.Queries.GetAll
{
    public sealed record GetRoleByUserIdQuery(string UserId) : IRequest<UserRolesManageDto>;
}
