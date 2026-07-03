using Application.Common.Interfaces.Identity;

namespace Application.Features.Authentication.Commands.ExternalLogin.Google;

public sealed class CreateGoogleLoginCommandHandler(
    IExternalAuthService _externalAuthService,
    ILoginPipeline _loginPipeline) :
    IRequestHandler<CreateGoogleLoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(CreateGoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _externalAuthService.GoogleLoginAsync(request.Dto);
        return await _loginPipeline.ExecuteAsync(user);
    }
}
