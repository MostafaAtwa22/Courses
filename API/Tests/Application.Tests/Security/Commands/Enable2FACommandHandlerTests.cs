using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Features.Security.Commands.Enable2FA;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Security.Commands;

public class Enable2FACommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    private readonly Enable2FACommandHandler _handler;

    public Enable2FACommandHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        _handler = new Enable2FACommandHandler(_userManagerMock.Object, _twoFactorServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldEnable2FA_WhenCodeIsValid()
    {
        // Arrange
        var user = new ApplicationUser { EmailConfirmed = true, TwoFactorEnabled = false };
        var command = new Enable2FACommand("123456") { User = user };

        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, command.Code)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.SetTwoFactorEnabledAsync(user, true)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.SetTwoFactorEnabledAsync(user, true), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_When2FAIsAlreadyEnabled()
    {
        // Arrange
        var user = new ApplicationUser { TwoFactorEnabled = true };
        var command = new Enable2FACommand("123456") { User = user };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("2FA is already enabled for this user.");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenEmailIsNotConfirmed()
    {
        // Arrange
        var user = new ApplicationUser { EmailConfirmed = false, TwoFactorEnabled = false };
        var command = new Enable2FACommand("123456") { User = user };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Email must be confirmed before enabling 2FA.");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenCodeIsInvalid()
    {
        // Arrange
        var user = new ApplicationUser { EmailConfirmed = true, TwoFactorEnabled = false };
        var command = new Enable2FACommand("invalid") { User = user };

        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, command.Code)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Invalid 2FA code.");
    }
}
