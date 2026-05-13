namespace Application.Features.Authentication.Commands.Login
{
    public sealed record CreateLoginCommand(LoginDto Dto) : IRequest<AuthResponseDto>;
}