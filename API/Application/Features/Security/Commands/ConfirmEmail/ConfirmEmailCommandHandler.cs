using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;

namespace Application.Features.Security.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(
    IUserIdentityService _userIdentityService,
    IPasswordService _passwordService,
    ITokenService _tokenService) :
    IRequestHandler<ConfirmEmailCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userIdentityService.FindUserByEmailAsync(request.Dto.Email)
            ?? throw new UnauthorizedException("Invalid verification attempt.");

        if (user.EmailConfirmed)
            return await _tokenService.GetAuthResponseAsync(user);

        if (await _userIdentityService.IsLockedOutAsync(user))
            throw new AccountLockedException("Your account is locked. Please try again later.");

        var succeeded = await _passwordService.ConfirmEmailAsync(user, request.Dto.Code);

        if (!succeeded)
        {
            await _userIdentityService.RecordFailedAccessAsync(user);
            throw new UnauthorizedException("Invalid verification code.");
        }

        return await _tokenService.GetAuthResponseAsync(user);
    }
}
