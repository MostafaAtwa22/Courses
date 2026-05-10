using Application.DTOs.Account;

namespace Application.Features.Account.Queries.GetById
{
    public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserResponseDto>;
}