namespace Application.Features.Account.Commands.Lock
{
    public sealed record LockUserCommand(Guid UserId, DateTimeOffset? LockoutUntil) : IRequest;
}