namespace Application.Features.Account.Commands.ForgetPassword
{
    public sealed class ForgetPasswordCommandHandler(
            IAuthService _authService, 
            IIdentityEmailService _identityEmailService) :
        IRequestHandler<ForgetPasswordCommand>
    {
        public async Task Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.FindUserByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("If the email exists, a reset link was sent.");

            var token = await _authService.GeneratePasswordResetTokenAsync(user);

            await _identityEmailService.SendPasswordResetEmailAsync(user, token);
        }
    }
}
