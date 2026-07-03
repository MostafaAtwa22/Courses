using Application.Common.Interfaces.Identity;

namespace Application.Features.Authentication.Commands.ExternalLogin.Facebook;

public sealed class CreateFacebookLoginCommandHandler(
    IExternalAuthService _externalAuthService,
    ILoginPipeline _loginPipeline) :
    IRequestHandler<CreateFacebookLoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(CreateFacebookLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _externalAuthService.FacebookLoginAsync(request.Dto);
        return await _loginPipeline.ExecuteAsync(user);
    }
}
