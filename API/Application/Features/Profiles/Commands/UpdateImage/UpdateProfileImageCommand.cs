using Application.Common.Interfaces.Identity;
using Application.DTOs.Profile;
using Domain.Constants;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.UpdateImage
{
    public sealed record UpdateProfileImageCommand(UpdateProfileImageDto Dto) : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }

    public sealed class UpdateProfileImageHandler(
        UserManager<ApplicationUser> _userManager,
        IFileService _fileService) :
    IRequestHandler<UpdateProfileImageCommand>
    {
        public async Task Handle(UpdateProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;

            // Delete old image if it exists
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                await _fileService.DeleteAsync(user.ProfilePictureUrl);
            }

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
