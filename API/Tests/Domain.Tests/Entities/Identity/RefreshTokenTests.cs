using Domain.Entities;
using Domain.Entities.Identity;
using FluentAssertions;

namespace Domain.Tests.Entities.Identity;

public class RefreshTokenTests
{
    [Fact]
    public void RefreshToken_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken();

        // Assert
        refreshToken.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void RefreshToken_ShouldHaveDefaultEmptyToken()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken();

        // Assert
        refreshToken.Token.Should().BeEmpty();
    }

    [Fact]
    public void RefreshToken_ShouldHaveDefaultEmptyJwtId()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken();

        // Assert
        refreshToken.JwtId.Should().BeEmpty();
    }

    [Fact]
    public void RefreshToken_ShouldHaveDefaultFalseIsUsed()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken();

        // Assert
        refreshToken.IsUsed.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_ShouldHaveDefaultFalseIsRevoked()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken();

        // Assert
        refreshToken.IsRevoked.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_ShouldHaveDefaultExpiryDate()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken();

        // Assert
        refreshToken.ExpiryDate.Should().Be(default(DateTime));
    }

    [Fact]
    public void RefreshToken_ShouldHaveDefaultEmptyUserId()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken();

        // Assert
        refreshToken.UserId.Should().BeEmpty();
    }

    [Fact]
    public void RefreshToken_ShouldAllowSettingToken()
    {
        // Arrange
        var expectedToken = "refresh_token_123";

        // Act
        var refreshToken = new RefreshToken { Token = expectedToken };

        // Assert
        refreshToken.Token.Should().Be(expectedToken);
    }

    [Fact]
    public void RefreshToken_ShouldAllowSettingJwtId()
    {
        // Arrange
        var expectedJwtId = "jwt_id_456";

        // Act
        var refreshToken = new RefreshToken { JwtId = expectedJwtId };

        // Assert
        refreshToken.JwtId.Should().Be(expectedJwtId);
    }

    [Fact]
    public void RefreshToken_ShouldAllowSettingIsUsed()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken { IsUsed = true };

        // Assert
        refreshToken.IsUsed.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_ShouldAllowSettingIsRevoked()
    {
        // Arrange & Act
        var refreshToken = new RefreshToken { IsRevoked = true };

        // Assert
        refreshToken.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_ShouldAllowSettingExpiryDate()
    {
        // Arrange
        var expectedExpiryDate = DateTime.UtcNow.AddDays(7);

        // Act
        var refreshToken = new RefreshToken { ExpiryDate = expectedExpiryDate };

        // Assert
        refreshToken.ExpiryDate.Should().Be(expectedExpiryDate);
    }

    [Fact]
    public void RefreshToken_ShouldAllowSettingUserId()
    {
        // Arrange
        var expectedUserId = "user123";

        // Act
        var refreshToken = new RefreshToken { UserId = expectedUserId };

        // Assert
        refreshToken.UserId.Should().Be(expectedUserId);
    }

    [Fact]
    public void RefreshToken_ShouldAllowSettingUser()
    {
        // Arrange
        var expectedUser = new ApplicationUser();

        // Act
        var refreshToken = new RefreshToken { User = expectedUser };

        // Assert
        refreshToken.User.Should().Be(expectedUser);
    }

    [Fact]
    public void RefreshToken_IsExpired_ShouldReturnTrueWhenExpired()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var isExpired = refreshToken.IsExpired;

        // Assert
        isExpired.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_IsExpired_ShouldReturnFalseWhenNotExpired()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var isExpired = refreshToken.IsExpired;

        // Assert
        isExpired.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_IsActive_ShouldReturnTrueWhenActive()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            IsRevoked = false,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var isActive = refreshToken.IsActive;

        // Assert
        isActive.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_IsActive_ShouldReturnFalseWhenRevoked()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            IsRevoked = true,
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var isActive = refreshToken.IsActive;

        // Assert
        isActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_IsActive_ShouldReturnFalseWhenExpired()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            IsRevoked = false,
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var isActive = refreshToken.IsActive;

        // Assert
        isActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_IsActive_ShouldReturnFalseWhenBothRevokedAndExpired()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            IsRevoked = true,
            ExpiryDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var isActive = refreshToken.IsActive;

        // Assert
        isActive.Should().BeFalse();
    }
}
