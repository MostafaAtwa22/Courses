using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Security;
using Application.Features.Security.Commands.Disable2FA;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Security.Commands;

public class Disable2FACommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    private readonly Disable2FACommandHandler _handler;

    public Disable2FACommandHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        _handler = new Disable2FACommandHandler(_userManagerMock.Object, _twoFactorServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDisable2FA_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new ApplicationUser { TwoFactorEnabled = true };
        var dto = new Disable2FADto { Password = "Password123!", Code = "123456" };
        var command = new Disable2FACommand(dto) { User = user };

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, dto.Code)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.SetTwoFactorEnabledAsync(user, false)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.SetTwoFactorEnabledAsync(user, false), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_When2FAIsAlreadyDisabled()
    {
        // Arrange
        var user = new ApplicationUser { TwoFactorEnabled = false };
        var dto = new Disable2FADto { Password = "Password123!", Code = "123456" };
        var command = new Disable2FACommand(dto) { User = user };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("2FA is already disabled for this user.");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new ApplicationUser { TwoFactorEnabled = true };
        var dto = new Disable2FADto { Password = "wrong", Code = "123456" };
        var command = new Disable2FACommand(dto) { User = user };

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Incorrect password.");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenCodeIsInvalid()
    {
        // Arrange
        var user = new ApplicationUser { TwoFactorEnabled = true };
        var dto = new Disable2FADto { Password = "Password123!", Code = "invalid" };
        var command = new Disable2FACommand(dto) { User = user };

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, dto.Code)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Invalid 2FA code.");
    }
}
