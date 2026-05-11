using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.UpdateImage
{
    public sealed class UpdateProfileImageCommandHandler(
        UserManager<ApplicationUser> _userManager,
        ICurrentUserService _currentUserService,
        IFileService _fileService) :
    IRequestHandler<UpdateProfileImageCommand>
    {
        public async Task Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("Login your account first");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException(nameof(ApplicationUser), Guid.Parse(userId));

            // Delete old image if it exists
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                await _fileService.DeleteAsync(user.ProfilePictureUrl);

            // Upload new image
            var file = request.Dto.Image;
            using var stream = file.OpenReadStream();
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            
            user.ProfilePictureUrl = await _fileService.UploadAsync(stream, fileName, FolderPaths.Users);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException("Failed to update profile image");
        }
    }
}
