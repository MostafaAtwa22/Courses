using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Features.Security.Commands.Generate2FA;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Security.Commands;

public class Generate2FATokenCommandHandlerTests
{
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    private readonly Generate2FATokenCommandHandler _handler;

    public Generate2FATokenCommandHandlerTests()
    {
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        _handler = new Generate2FATokenCommandHandler(_twoFactorServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSendOtp_WhenUserIsValid()
    {
        // Arrange
        var user = new ApplicationUser { EmailConfirmed = true, TwoFactorEnabled = false };
        var command = new Generate2FATokenCommand() { User = user };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _twoFactorServiceMock.Verify(x => x.SendOtpAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_When2FAIsAlreadyEnabled()
    {
        // Arrange
        var user = new ApplicationUser { TwoFactorEnabled = true };
        var command = new Generate2FATokenCommand() { User = user };

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
        var command = new Generate2FATokenCommand() { User = user };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Email must be confirmed before enabling 2FA.");
    }
}
