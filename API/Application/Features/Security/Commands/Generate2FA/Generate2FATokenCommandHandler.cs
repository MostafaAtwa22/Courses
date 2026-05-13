using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;

namespace Application.Features.Security.Commands.Generate2FA
{
    public sealed class Generate2FATokenCommandHandler(
        ITwoFactorService _twoFactorService) 
        : IRequestHandler<Generate2FATokenCommand>
    {
        public async Task Handle(Generate2FATokenCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;
            if (user.TwoFactorEnabled)
                throw new BadRequestException("2FA is already enabled for this user.");
            
            if (!user.EmailConfirmed)
                throw new BadRequestException("Email must be confirmed before enabling 2FA.");

            await _twoFactorService.SendOtpAsync(user);
        }
    }
}
