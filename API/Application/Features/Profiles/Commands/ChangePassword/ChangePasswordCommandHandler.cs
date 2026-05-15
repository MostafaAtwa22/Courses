namespace Application.Features.Profiles.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(
        IAuthService _authService) 
        : IRequestHandler<ChangePasswordCommand>
    {
        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;

            var result = await _authService.ChangePasswordAsync(user, request.Dto.OldPassword, request.Dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to change password");
            
            await _authService.UpdateSecurityStampAsync(user);
        }
    }
}
