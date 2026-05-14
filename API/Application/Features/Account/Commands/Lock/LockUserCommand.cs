using Application.DTOs.Account;

namespace Application.Features.Account.Commands.Lock
{
    public sealed record LockUserCommand(Guid UserId, LockUserDto Dto) : IRequest;
}