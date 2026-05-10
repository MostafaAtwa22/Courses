using Application.DTOs.Account;

namespace Application.Features.Account.Queries.GetAll
{
    public sealed record GetUsersQuery(UserQueryParams Params) : IRequest<PaginatedResult<UserResponseDto>>;
}