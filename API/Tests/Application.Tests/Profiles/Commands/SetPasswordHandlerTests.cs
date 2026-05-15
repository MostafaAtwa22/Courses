using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.SetPassword;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Profiles.Commands;

public class SetPasswordHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly SetPasswordCommandHandler _handler;

    public SetPasswordHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new SetPasswordCommandHandler(_authServiceMock.Object);
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

        _authServiceMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(false);
        _authServiceMock.Setup(x => x.SetPasswordAsync(user, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _authServiceMock.Verify(x => x.SetPasswordAsync(user, dto.NewPassword), Times.Once);
        _authServiceMock.Verify(x => x.UpdateSecurityStampAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUserAlreadyHasPassword()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new SetPasswordDto
        {
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new SetPasswordCommand(dto) { User = user };

        _authServiceMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(true);

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
        var dto = new SetPasswordDto
        {
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new SetPasswordCommand(dto) { User = user };

        _authServiceMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(false);
        var identityError = new IdentityError { Description = "Password too short" };
        _authServiceMock.Setup(x => x.SetPasswordAsync(user, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Password too short");
    }
}
