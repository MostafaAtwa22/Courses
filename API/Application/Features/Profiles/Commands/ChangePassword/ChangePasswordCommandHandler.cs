namespace Application.Features.Profiles.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(
        IPasswordService _passwordService,
        IUserIdentityService _userIdentityService,
        IRefreshTokenRepository _refreshTokenRepository) 
        : IRequestHandler<ChangePasswordCommand>
    {
        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;

            var result = await _passwordService.ChangePasswordAsync(user, request.Dto.OldPassword, request.Dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to change password");
            
            await _userIdentityService.UpdateSecurityStampAsync(user);
            
            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);
        }
    }
}
