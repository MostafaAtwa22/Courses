using Application.DTOs.Authentication;
using MediatR;

namespace Application.Features.Authentication.Commands.ExternalLogin.Github;

public sealed record CreateGithubLoginCommand(GithubLoginDto Dto) : IRequest<AuthResponseDto>;
