namespace Application.Features.Account.Commands.UnLock
{
    public sealed record UnLockUserCommand(Guid UserId) : IRequest;
}