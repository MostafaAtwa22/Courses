using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.SetPassword
{
    public sealed class SetPasswordCommandHandler(
        UserManager<ApplicationUser> _userManager) 
        : IRequestHandler<SetPasswordCommand>
    {
        public async Task Handle(SetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;
            if(await _userManager.HasPasswordAsync(user))
                throw new BadRequestException("User already has a password");

            var result = await _userManager.AddPasswordAsync(user, request.Dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to set password");
        }
    }
}
