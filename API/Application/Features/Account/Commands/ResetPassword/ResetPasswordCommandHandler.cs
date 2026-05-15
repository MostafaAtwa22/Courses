using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Features.Account.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(
            IAuthService _authService) :
        IRequestHandler<ResetPasswordCommand>
    {
        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.FindUserByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("Invalid verification attempt.");

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(request.Dto.Token));

            var result = await _authService.ResetPasswordAsync(user, decodedToken, request.Dto.NewPassword);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Password reset failed. Please ensure the token is valid and try again.");
            
            await _authService.UpdateSecurityStampAsync(user);
        }
    }
}
