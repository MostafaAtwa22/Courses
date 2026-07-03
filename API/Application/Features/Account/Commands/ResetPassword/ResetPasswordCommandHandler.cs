using System.Text;
using Application.Common.Interfaces.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Features.Account.Commands.ResetPassword
{
    public sealed class ResetPasswordCommandHandler(
            IUserIdentityService _userIdentityService,
            IPasswordService _passwordService) :
        IRequestHandler<ResetPasswordCommand>
    {
        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userIdentityService.FindUserByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("Invalid verification attempt.");

            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(request.Dto.Token));

            var result = await _passwordService.ResetPasswordAsync(user, decodedToken, request.Dto.NewPassword);

            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Password reset failed. Please ensure the token is valid and try again.");
            
            await _userIdentityService.UpdateSecurityStampAsync(user);
        }
    }
}
