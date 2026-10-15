using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.Delete
{
    public sealed class DeleteProfileHandler(
        UserManager<ApplicationUser> _userManager,
        IRefreshTokenRepository _refreshTokenRepository) :
    IRequestHandler<DeleteProfileCommand>
    {
        public async Task Handle(DeleteProfileCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;
            
            var result = await _userManager.CheckPasswordAsync(user, request.Dto.Password);
            if (!result)
                throw new BadRequestException("Invalid password");

            await _userManager.UpdateSecurityStampAsync(user);
            
            await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id, cancellationToken);
            
            var deletedResult = await _userManager.DeleteAsync(user);

            if (!deletedResult.Succeeded)
                throw new BadRequestException("Failed to delete user");
        }
    }
}
