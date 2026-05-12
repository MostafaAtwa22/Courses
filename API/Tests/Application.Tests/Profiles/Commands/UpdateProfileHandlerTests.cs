using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.Update;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Profiles.Commands;

public class UpdateProfileHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly UpdateUserProfileHandler _handler;

    public UpdateProfileHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _authServiceMock = new Mock<IAuthService>();
        _handler = new UpdateUserProfileHandler(_userManagerMock.Object, _authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenDataIsValid()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id", UserName = "olduser" };
        var dto = new UpdateProfileDto
        {
            FirstName = "New",
            LastName = "Name",
            UserName = "newuser"
        };
        var command = new UpdateProfileCommand(dto) { User = user };

        _authServiceMock.Setup(x => x.IsUserNameExistsAsync(dto.UserName)).ReturnsAsync(false);
        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.FirstName.Should().Be("New");
        user.LastName.Should().Be("Name");
        user.UserName.Should().Be("newuser");
        _userManagerMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUserNameExists()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id", UserName = "olduser" };
        var dto = new UpdateProfileDto { UserName = "takenuser" };
        var command = new UpdateProfileCommand(dto) { User = user };

        _authServiceMock.Setup(x => x.IsUserNameExistsAsync(dto.UserName)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Username is already taken");
    }
}
