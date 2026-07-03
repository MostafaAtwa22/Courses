using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using MediatR;

namespace Application.Features.Authentication.Commands.ExternalLogin.Github;

public sealed class CreateGithubLoginCommandHandler(
    IExternalAuthService externalAuthService,
    ILoginPipeline loginPipeline) : 
    IRequestHandler<CreateGithubLoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(CreateGithubLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await externalAuthService.GithubLoginAsync(request.Dto);
        return await loginPipeline.ExecuteAsync(user);
    }
}
