using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.RefreshToken;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Tests.Authentication.Commands;

public class CreateRefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly CreateRefreshTokenCommandHandler _handler;

    public CreateRefreshTokenCommandHandlerTests()
    {
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _tokenServiceMock = new Mock<ITokenService>();

        _handler = new CreateRefreshTokenCommandHandler(
            _refreshTokenRepositoryMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenTokenIsValid()
    {
        // Arrange
        var token = "valid_refresh_token";
        var accessToken = CreateMockJwtToken("stamp123");
        var command = new CreateRefreshTokenCommand(token, accessToken);
        var user = new ApplicationUser { Email = "test@example.com", SecurityStamp = "stamp123" };
        var storedToken = new RefreshToken
        {
            Token = token,
            IsUsed = false,
            IsRevoked = false,
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            User = user
        };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "new_jwt_token" };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);
        _tokenServiceMock.Setup(x => x.GenerateAuthWithRefreshTokenAsync(user))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
        storedToken.IsUsed.Should().BeTrue();
        _refreshTokenRepositoryMock.Verify(x => x.UpdateAsync(storedToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenNotFound()
    {
        // Arrange
        var token = "invalid_token";
        var accessToken = CreateMockJwtToken("stamp123");
        var command = new CreateRefreshTokenCommand(token, accessToken);

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
        var accessToken = CreateMockJwtToken("stamp123");
        var command = new CreateRefreshTokenCommand(token, accessToken);
        var user = new ApplicationUser { Email = "test@example.com", SecurityStamp = "stamp123" };
        var storedToken = new RefreshToken
        {
            Token = token,
            IsUsed = false,
            IsRevoked = false,
            ExpiryDate = DateTime.UtcNow.AddDays(-1),
            User = user
        };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Refresh token is expired or revoked");
        storedToken.IsRevoked.Should().BeTrue();
        _refreshTokenRepositoryMock.Verify(x => x.UpdateAsync(storedToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenTokenIsRevoked()
    {
        // Arrange
        var token = "revoked_token";
        var accessToken = CreateMockJwtToken("stamp123");
        var command = new CreateRefreshTokenCommand(token, accessToken);
        var user = new ApplicationUser { Email = "test@example.com", SecurityStamp = "stamp123" };
        var storedToken = new RefreshToken
        {
            Token = token,
            IsUsed = false,
            IsRevoked = true,
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            User = user
        };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Refresh token is expired or revoked");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserIsNull()
    {
        // Arrange
        var token = "token_without_user";
        var accessToken = CreateMockJwtToken("stamp123");
        var command = new CreateRefreshTokenCommand(token, accessToken);
        var storedToken = new RefreshToken
        {
            Token = token,
            IsUsed = false,
            IsRevoked = false,
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            User = null!
        };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid refresh token");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenSecurityStampMismatch()
    {
        // Arrange
        var token = "valid_token";
        var accessToken = CreateMockJwtToken("old_stamp");
        var command = new CreateRefreshTokenCommand(token, accessToken);
        var user = new ApplicationUser { Email = "test@example.com", SecurityStamp = "new_stamp" };
        var storedToken = new RefreshToken
        {
            Token = token,
            IsUsed = false,
            IsRevoked = false,
            ExpiryDate = DateTime.UtcNow.AddDays(1),
            User = user
        };

        _refreshTokenRepositoryMock.Setup(x => x.GetByTokenAsync(token))
            .ReturnsAsync(storedToken);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Session has been invalidated. Please log in again.");
        storedToken.IsRevoked.Should().BeTrue();
        _refreshTokenRepositoryMock.Verify(x => x.UpdateAsync(storedToken), Times.Once);
    }

    private string CreateMockJwtToken(string securityStamp)
    {
        var claims = new List<Claim>
        {
            new Claim("security_stamp", securityStamp),
            new Claim(JwtRegisteredClaimNames.Sub, "user123"),
            new Claim(JwtRegisteredClaimNames.Email, "test@example.com")
        };

        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("super_secret_key_for_testing_12345678"));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(
            key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "test",
            audience: "test",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
