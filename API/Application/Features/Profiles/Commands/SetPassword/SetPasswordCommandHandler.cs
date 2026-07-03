using Application.Common.Interfaces.Identity;

namespace Application.Features.Profiles.Commands.SetPassword
{
    public sealed class SetPasswordCommandHandler(
        IPasswordService _passwordService,
        IUserIdentityService _userIdentityService) 
        : IRequestHandler<SetPasswordCommand>
    {
        public async Task Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;
            if(await _passwordService.HasPasswordAsync(user))
                throw new BadRequestException("User already has a password");

            var result = await _passwordService.SetPasswordAsync(user, request.Dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to set password");

            await _userIdentityService.UpdateSecurityStampAsync(user);
        }
    }
}
