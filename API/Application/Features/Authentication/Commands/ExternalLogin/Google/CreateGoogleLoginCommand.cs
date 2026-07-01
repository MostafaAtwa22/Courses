namespace Application.Features.Authentication.Commands.ExternalLogin.Google
{
    public sealed record CreateGoogleLoginCommand(GoogleLoginDto Dto) : IRequest<AuthResponseDto>;
}