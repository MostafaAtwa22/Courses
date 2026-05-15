using Application.Common.Exceptions;

namespace Application.Features.Security.Commands.ConfirmEmail
{
    public sealed class ConfirmEmailCommandHandler(
            IAuthService _authService) :
        IRequestHandler<ConfirmEmailCommand, AuthResponseDto>
    {
        public async Task<AuthResponseDto> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _authService.FindUserByEmailAsync(request.Dto.Email)
                ?? throw new UnauthorizedException("Invalid verification attempt.");

            if (user.EmailConfirmed)
            {
                return await _authService.GetAuthResponseAsync(user);
            }

            if (await _authService.IsLockedOutAsync(user))
                throw new AccountLockedException("Your account is locked. Please try again later.");

            var succeeded = await _authService.ConfirmEmailAsync(user, request.Dto.Code);
            
            if (!succeeded)
            {
                await _authService.RecordFailedAccessAsync(user); 
                throw new UnauthorizedException("Invalid verification code.");
            }
            
            return await _authService.GetAuthResponseAsync(user);
        }
    }
}
