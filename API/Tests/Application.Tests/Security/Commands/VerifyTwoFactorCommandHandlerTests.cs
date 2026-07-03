using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.DTOs.Security;
using Application.Features.Security.Commands.VerifyTwoFactor;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;namespace Application.Tests.Security.Commands;

public class VerifyTwoFactorCommandHandlerTests
{
    private readonly Mock<IUserIdentityService> _userIdentityServiceMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly VerifyTwoFactorCommandHandler _handler;

    public VerifyTwoFactorCommandHandlerTests()
    {
        _userIdentityServiceMock = new Mock<IUserIdentityService>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new VerifyTwoFactorCommandHandler(_userIdentityServiceMock.Object, _twoFactorServiceMock.Object, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenOtpIsValid()
    {
        // Arrange
        var dto = new VerifyTwoFactorDto { Email = "test@example.com", Code = "123456" };
        var user = new ApplicationUser { Email = dto.Email, TwoFactorEnabled = true };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt" };

        _userIdentityServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, dto.Code)).ReturnsAsync(true);
        _tokenServiceMock.Setup(x => x.GenerateAuthWithRefreshTokenAsync(user)).ReturnsAsync(authResponse);

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
        _userIdentityServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

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
        _userIdentityServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

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
        _userIdentityServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);

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

        _userIdentityServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _twoFactorServiceMock.Setup(x => x.VerifyOtpAsync(user, dto.Code)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(new VerifyTwoFactorCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid verification code.");
        _userIdentityServiceMock.Verify(x => x.RecordFailedAccessAsync(user), Times.Once);
    }
}
