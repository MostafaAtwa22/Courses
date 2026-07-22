using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Profiles.Commands.DeleteImage;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Profiles.Commands;

public class DeleteProfileImageCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly DeleteProfileImageCommandHandler _handler;

    public DeleteProfileImageCommandHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _fileServiceMock = new Mock<IFileService>();

        _handler = new DeleteProfileImageCommandHandler(
            _userManagerMock.Object,
            _currentUserServiceMock.Object,
            _fileServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteImage_WhenUserHasProfileImage()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var profileImageUrl = "https://example.com/profile.jpg";
        var command = new DeleteProfileImageCommand();
        var user = new ApplicationUser 
        { 
            Id = userId,
            ProfilePictureUrl = profileImageUrl
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(x => x.DeleteAsync(profileImageUrl), Times.Once);
        user.ProfilePictureUrl.Should().BeNull();
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEarly_WhenUserHasNoProfileImage()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var command = new DeleteProfileImageCommand();
        var user = new ApplicationUser 
        { 
            Id = userId,
            ProfilePictureUrl = null
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Never);
        _userManagerMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        var command = new DeleteProfileImageCommand();

        _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Login your account first");
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var command = new DeleteProfileImageCommand();

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUpdateFails()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var profileImageUrl = "https://example.com/profile.jpg";
        var command = new DeleteProfileImageCommand();
        var user = new ApplicationUser 
        { 
            Id = userId,
            ProfilePictureUrl = profileImageUrl
        };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>()
            .WithMessage("Failed to delete profile image");
    }
}
