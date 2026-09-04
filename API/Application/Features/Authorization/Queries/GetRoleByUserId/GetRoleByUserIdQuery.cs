namespace Application.Features.Authorization.Queries.GetRoleByUserId
{
    public sealed record GetRoleByUserIdQuery(string UserId) : IRequest<UserRolesResponseDto>;
}
