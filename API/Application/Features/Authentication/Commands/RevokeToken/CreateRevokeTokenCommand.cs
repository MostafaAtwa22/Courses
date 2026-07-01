using MediatR;

namespace Application.Features.Authentication.Commands.RevokeToken
{
    public sealed record CreateRevokeTokenCommand(string Token) : IRequest;
}
