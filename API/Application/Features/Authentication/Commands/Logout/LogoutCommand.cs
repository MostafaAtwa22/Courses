using MediatR;

namespace Application.Features.Authentication.Commands.Logout;

public sealed record LogoutCommand(string Token) : IRequest;
