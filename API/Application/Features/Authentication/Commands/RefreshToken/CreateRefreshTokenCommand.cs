using Application.DTOs.Authentication;
using MediatR;

namespace Application.Features.Authentication.Commands.RefreshToken
{
    public sealed record CreateRefreshTokenCommand(string Token) : IRequest<AuthResponseDto>;
}
