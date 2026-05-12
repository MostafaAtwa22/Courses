using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler(
        UserManager<ApplicationUser> _userManager) 
        : IRequestHandler<ChangePasswordCommand>
    {
        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;

            var result = await _userManager.ChangePasswordAsync(user, request.Dto.OldPassword, request.Dto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.FirstOrDefault()?.Description ?? "Failed to change password");
        }
    }
}
