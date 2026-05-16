using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.DTOs.Security;
using Application.Features.Security.Commands.ConfirmEmail;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System.Text;

namespace Application.Tests.Security.Commands;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly ConfirmEmailCommandHandler _handler;

    public ConfirmEmailCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new ConfirmEmailCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenConfirmationIsSuccessful()
    {
        // Arrange
        var token = "raw-token";
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var dto = new ConfirmEmailDto { Email = "test@example.com", Code = encodedToken };
        var user = new ApplicationUser { Email = dto.Email, EmailConfirmed = false };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt" };

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _authServiceMock.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(true);
        _authServiceMock.Setup(x => x.GetAuthResponseAsync(user)).ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(new ConfirmEmailCommand(dto), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponseImmediately_WhenEmailIsAlreadyConfirmed()
    {
        // Arrange
        var dto = new ConfirmEmailDto { Email = "test@example.com", Code = "any" };
        var user = new ApplicationUser { Email = dto.Email, EmailConfirmed = true };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt" };

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.GetAuthResponseAsync(user)).ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(new ConfirmEmailCommand(dto), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
        _authServiceMock.Verify(x => x.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var dto = new ConfirmEmailDto { Email = "notfound@example.com", Code = "any" };
        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(new ConfirmEmailCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid verification attempt.");
    }

    [Fact]
    public async Task Handle_ShouldThrowAccountLocked_WhenUserIsLockedOut()
    {
        // Arrange
        var dto = new ConfirmEmailDto { Email = "locked@example.com", Code = "any" };
        var user = new ApplicationUser { Email = dto.Email, EmailConfirmed = false };
        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(new ConfirmEmailCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountLockedException>().WithMessage("Your account is locked. Please try again later.");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenConfirmationFails()
    {
        // Arrange
        var token = "raw-token";
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var dto = new ConfirmEmailDto { Email = "test@example.com", Code = encodedToken };
        var user = new ApplicationUser { Email = dto.Email, EmailConfirmed = false };

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _authServiceMock.Setup(x => x.ConfirmEmailAsync(user, token)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(new ConfirmEmailCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid verification code.");
        _authServiceMock.Verify(x => x.RecordFailedAccessAsync(user), Times.Once);
    }
}
