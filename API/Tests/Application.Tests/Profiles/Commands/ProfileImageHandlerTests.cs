using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.UpdateImage;
using Application.Features.Profiles.Commands.DeleteImage;
using Domain.Constants;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Profiles.Commands;

public class ProfileImageHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly UpdateProfileImageHandler _updateHandler;
    private readonly DeleteProfileImageHandler _deleteHandler;

    public ProfileImageHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _fileServiceMock = new Mock<IFileService>();
        _updateHandler = new UpdateProfileImageHandler(_userManagerMock.Object, _fileServiceMock.Object);
        _deleteHandler = new DeleteProfileImageHandler(_userManagerMock.Object, _fileServiceMock.Object);
    }

    [Fact]
    public async Task UpdateHandle_ShouldUpdateImage_WhenFileIsValid()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id", ProfilePictureUrl = "old.jpg" };
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(x => x.FileName).Returns("test.jpg");
        fileMock.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());
        
        var dto = new UpdateProfileImageDto { Image = fileMock.Object };
        var command = new UpdateProfileImageCommand(dto) { User = user };

        _fileServiceMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("new.jpg");
        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _updateHandler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(x => x.DeleteAsync("old.jpg"), Times.Once);
        user.ProfilePictureUrl.Should().Be("new.jpg");
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteHandle_ShouldDeleteImage_WhenUserHasImage()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id", ProfilePictureUrl = "image.jpg" };
        var command = new DeleteProfileImageCommand() { User = user };

        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _deleteHandler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(x => x.DeleteAsync("image.jpg"), Times.Once);
        user.ProfilePictureUrl.Should().BeNull();
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }
}
