using System.Text;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Application.Common.Options;

namespace Application.Features.Account.Commands.ForgetPassword
{
    public sealed class ForgetPasswordCommandHandler(
            IAuthService _authService, 
            IIdentityEmailService _identityEmailService,
            IOptions<UrlsOptions> _urlsOptions) :
        IRequestHandler<ForgetPasswordCommand>
    {
        public async Task Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.FindUserByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("If the email exists, a reset link was sent.");

            var token = await _authService.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token)
            );

            var baseUrl = _urlsOptions.Value.Client;
            var resetLink = $"{baseUrl}/authentication/reset-password?email={user.Email}&token={encodedToken}";

            await _identityEmailService.SendPasswordResetEmailAsync(user, resetLink);
        }
    }
}
