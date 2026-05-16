using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Account;
using Application.Features.Account.Commands.ForgetPassword;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Account.Commands;

public class ForgetPasswordCommandHandlerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IIdentityEmailService> _identityEmailServiceMock;
    private readonly ForgetPasswordCommandHandler _handler;

    public ForgetPasswordCommandHandlerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _identityEmailServiceMock = new Mock<IIdentityEmailService>();
        _handler = new ForgetPasswordCommandHandler(_authServiceMock.Object, _identityEmailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSendResetEmail_WhenUserExists()
    {
        // Arrange
        var dto = new ForgetPasswordDto { Email = "test@example.com" };
        var user = new ApplicationUser { Email = dto.Email };
        var token = "reset-token";

        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync(user);
        _authServiceMock.Setup(x => x.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(token);

        // Act
        await _handler.Handle(new ForgetPasswordCommand(dto), CancellationToken.None);

        // Assert
        _identityEmailServiceMock.Verify(x => x.SendPasswordResetEmailAsync(user, token), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var dto = new ForgetPasswordDto { Email = "notfound@example.com" };
        _authServiceMock.Setup(x => x.FindUserByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(new ForgetPasswordCommand(dto), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("If the email exists, a reset link was sent.");
    }
}
