namespace Application.Features.Authentication.Commands.ExternalLogin.Facebook
{
    public sealed record CreateFacebookLoginCommand(FacebookLoginDto Dto) : IRequest<AuthResponseDto>;
}