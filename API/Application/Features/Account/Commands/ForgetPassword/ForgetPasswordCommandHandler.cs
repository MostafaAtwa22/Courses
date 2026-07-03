using Application.Common.Interfaces.Identity;

namespace Application.Features.Account.Commands.ForgetPassword
{
    public sealed class ForgetPasswordCommandHandler(
            IUserIdentityService _userIdentityService,
            IPasswordService _passwordService,
            IIdentityEmailService _identityEmailService) :
        IRequestHandler<ForgetPasswordCommand>
    {
        public async Task Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userIdentityService.FindUserByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("If the email exists, a reset link was sent.");

            var token = await _passwordService.GeneratePasswordResetTokenAsync(user);

            await _identityEmailService.SendPasswordResetEmailAsync(user, token);
        }
    }
}
