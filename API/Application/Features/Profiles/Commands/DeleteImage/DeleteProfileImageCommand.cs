using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Profiles.Commands.DeleteImage
{
    public sealed record DeleteProfileImageCommand() : IRequest, ICurrentUserRequest
    {
        public ApplicationUser? User { get; set; }
    }

    public sealed class DeleteProfileImageHandler(
        UserManager<ApplicationUser> _userManager,
        IFileService _fileService) :
    IRequestHandler<DeleteProfileImageCommand>
    {
        public async Task Handle(DeleteProfileImageCommand request, CancellationToken cancellationToken)
        {
            var user = request.User!;

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
