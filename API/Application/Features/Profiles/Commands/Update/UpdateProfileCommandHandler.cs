using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.Update
{
    public sealed class UpdateUserProfileHandler(
        UserManager<ApplicationUser> _userManager,
        IAuthService _authService) :
    IRequestHandler<UpdateProfileCommand>
    {
        public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;

            if (user.UserName != request.Dto.UserName && await _authService.IsUserNameExistsAsync(request.Dto.UserName))
                throw new BadRequestException("Username is already taken");

            user.UpdateFromDto(request.Dto);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException("Failed to update user");
        }
    }
}
