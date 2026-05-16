using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.DTOs.Security;
using Application.Features.Security.Commands.VerifyTwoFactor;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Security.Commands;

public class VerifyTwoFactorCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    private readonly VerifyTwoFactorCommandHandler _handler;

    public VerifyTwoFactorCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        _handler = new VerifyTwoFactorCommandHandler(_authServiceMock.Object, _twoFactorServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenOtpIsValid()
    {
        // Arrange
        var dto = new VerifyTwoFactorDto { Email = "test@example.com", Code = "123456" };
        var user = new ApplicationUser { Email = dto.Email, TwoFactorEnabled = true };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt" };

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, dto.Code)).ReturnsAsync(true);
        _authServiceMock.Setup(x => x.GetAuthResponseAsync(user)).ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(new VerifyTwoFactorCommand(dto), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var dto = new VerifyTwoFactorDto { Email = "notfound@example.com", Code = "123456" };
        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(new VerifyTwoFactorCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid verification attempt.");
    }

    [Fact]
    public async Task Handle_ShouldThrowAccountLocked_WhenUserIsLockedOut()
    {
        // Arrange
        var dto = new VerifyTwoFactorDto { Email = "locked@example.com", Code = "123456" };
        var user = new ApplicationUser { Email = dto.Email, TwoFactorEnabled = true };
        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(new VerifyTwoFactorCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountLockedException>().WithMessage("Your account is locked. Please try again later.");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_When2FAIsNotEnabled()
    {
        // Arrange
        var dto = new VerifyTwoFactorDto { Email = "test@example.com", Code = "123456" };
        var user = new ApplicationUser { Email = dto.Email, TwoFactorEnabled = false };
        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(new VerifyTwoFactorCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Two-factor authentication is not enabled for this account.");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenOtpIsInvalid()
    {
        // Arrange
        var dto = new VerifyTwoFactorDto { Email = "test@example.com", Code = "invalid" };
        var user = new ApplicationUser { Email = dto.Email, TwoFactorEnabled = true };

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, dto.Code)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(new VerifyTwoFactorCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid verification code.");
        _authServiceMock.Verify(x => x.RecordFailedAccessAsync(user), Times.Once);
    }
}
