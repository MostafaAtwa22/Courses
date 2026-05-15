namespace Application.Features.Profiles.Commands.SetPassword
{
    public sealed class SetPasswordCommandHandler(
        IAuthService _authService) 
        : IRequestHandler<SetPasswordCommand>
    {
        public async Task Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;
            if(await _authService.HasPasswordAsync(user))
                throw new BadRequestException("User already has a password");

            var result = await _authService.SetPasswordAsync(user, request.Dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to set password");

            await _authService.UpdateSecurityStampAsync(user);
        }
    }
}
