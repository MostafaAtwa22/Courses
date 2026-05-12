using Application.Common.Exceptions;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.SetPassword;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Profiles.Commands;

public class SetPasswordHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly SetPasswordCommandHandler _handler;

    public SetPasswordHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _handler = new SetPasswordCommandHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSetPassword_WhenUserHasNoPassword()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new SetPasswordDto
        {
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new SetPasswordCommand(dto) { User = user };

        _userManagerMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(false);
        _userManagerMock.Setup(x => x.AddPasswordAsync(user, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.HasPasswordAsync(user), Times.Once);
        _userManagerMock.Verify(x => x.AddPasswordAsync(user, dto.NewPassword), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUserAlreadyHasPassword()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new SetPasswordDto { NewPassword = "NewPassword123!" };
        var command = new SetPasswordCommand(dto) { User = user };

        _userManagerMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User already has a password");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenIdentityFails()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new SetPasswordDto { NewPassword = "NewPassword123!" };
        var command = new SetPasswordCommand(dto) { User = user };

        _userManagerMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(false);
        var identityError = new IdentityError { Description = "Password too short" };
        _userManagerMock.Setup(x => x.AddPasswordAsync(user, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Password too short");
    }
}
