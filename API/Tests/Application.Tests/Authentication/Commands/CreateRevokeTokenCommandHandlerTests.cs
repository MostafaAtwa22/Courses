using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Features.Authentication.Commands.RevokeToken;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Authentication.Commands;

public class CreateRevokeTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly CreateRevokeTokenCommandHandler _handler;

    public CreateRevokeTokenCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _handler = new CreateRevokeTokenCommandHandler(_refreshTokenRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRevokeToken_WhenTokenIsValid()
    {
        // Arrange
        var token = "valid_token";
        var command = new CreateRevokeTokenCommand(token);
        var storedToken = new RefreshToken 
        { 
            Token = token, 
            IsUsed = false, 
            IsRevoked = false, 
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        storedToken.IsRevoked.Should().BeTrue();
        _refreshTokenRepositoryMock.Verify(x => x.UpdateAsync(storedToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenNotFound()
    {
        // Arrange
        var token = "invalid_token";
        var command = new CreateRevokeTokenCommand(token);

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync((RefreshToken?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid refresh token");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenIsExpired()
    {
        // Arrange
        var token = "expired_token";
        var command = new CreateRevokeTokenCommand(token);
        var storedToken = new RefreshToken 
        { 
            Token = token, 
            IsUsed = false, 
            IsRevoked = false, 
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Refresh token is already expired or revoked");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenIsAlreadyRevoked()
    {
        // Arrange
        var token = "already_revoked_token";
        var command = new CreateRevokeTokenCommand(token);
        var storedToken = new RefreshToken 
        { 
            Token = token, 
            IsUsed = false, 
            IsRevoked = true, 
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Refresh token is already expired or revoked");
    }
}
