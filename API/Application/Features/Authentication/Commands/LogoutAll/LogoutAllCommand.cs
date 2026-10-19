using MediatR;

namespace Application.Features.Authentication.Commands.LogoutAll;

public sealed record LogoutAllCommand() : IRequest;
