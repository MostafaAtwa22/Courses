using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Account;
using Application.Features.Account.Commands.ResetPassword;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System.Text;

namespace Application.Tests.Account.Commands;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _handler = new ResetPasswordCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldResetPassword_WhenTokenIsValid()
    {
        // Arrange
        var token = "raw-token";
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var dto = new ResetPasswordDto 
        { 
            Email = "test@example.com", 
            Token = encodedToken, 
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var user = new ApplicationUser { Email = dto.Email };

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.ResetPasswordAsync(user, token, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(new ResetPasswordCommand(dto), CancellationToken.None);

        // Assert
        _authServiceMock.Verify(x => x.ResetPasswordAsync(user, token, dto.NewPassword), Times.Once);
        _authServiceMock.Verify(x => x.UpdateSecurityStampAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var dto = new ResetPasswordDto { Email = "notfound@example.com", Token = "any" };
        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(new ResetPasswordCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid verification attempt.");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenResetFails()
    {
        // Arrange
        var token = "raw-token";
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var dto = new ResetPasswordDto { Email = "test@example.com", Token = encodedToken, NewPassword = "New" };
        var user = new ApplicationUser { Email = dto.Email };

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.ResetPasswordAsync(user, token, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

        // Act
        Func<Task> act = async () => await _handler.Handle(new ResetPasswordCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Invalid token");
    }
}
