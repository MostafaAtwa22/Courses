using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.DeleteImage
{
    public sealed class DeleteProfileImageCommandHandler(
        UserManager<ApplicationUser> _userManager,
        ICurrentUserService _currentUserService,
        IFileService _fileService) :
    IRequestHandler<DeleteProfileImageCommand>
    {
        public async Task Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("Login your account first");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(userId));

            if (string.IsNullOrEmpty(user.ProfilePictureUrl))
                return;

            // Delete image from storage
            await _fileService.DeleteAsync(user.ProfilePictureUrl);

            // Clear URL in DB
            user.ProfilePictureUrl = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException("Failed to delete profile image");
        }
    }
}
