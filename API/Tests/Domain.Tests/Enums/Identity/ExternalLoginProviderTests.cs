using Domain.Enums.Identity;
using FluentAssertions;

namespace Domain.Tests.Enums.Identity;

public class ExternalLoginProviderTests
{
    [Fact]
    public void ExternalLoginProvider_ShouldHaveGoogleValue()
    {
        // Arrange & Act
        var provider = ExternalLoginProvider.Google;

        // Assert
        provider.Should().Be(ExternalLoginProvider.Google);
    }

    [Fact]
    public void ExternalLoginProvider_ShouldHaveFacebookValue()
    {
        // Arrange & Act
        var provider = ExternalLoginProvider.Facebook;

        // Assert
        provider.Should().Be(ExternalLoginProvider.Facebook);
    }

    [Fact]
    public void ExternalLoginProvider_ShouldHaveGithubValue()
    {
        // Arrange & Act
        var provider = ExternalLoginProvider.Github;

        // Assert
        provider.Should().Be(ExternalLoginProvider.Github);
    }

    [Fact]
    public void ExternalLoginProvider_ShouldHaveThreeValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<ExternalLoginProvider>();

        // Assert
        values.Should().HaveCount(3);
    }

    [Fact]
    public void ExternalLoginProvider_ShouldBeComparable()
    {
        // Arrange & Act
        var google = ExternalLoginProvider.Google;
        var facebook = ExternalLoginProvider.Facebook;
        var github = ExternalLoginProvider.Github;

        // Assert
        google.Should().NotBe(facebook);
        facebook.Should().NotBe(github);
        google.Should().NotBe(github);
    }

    [Fact]
    public void ExternalLoginProvider_ShouldBeAssignableToInt()
    {
        // Arrange & Act
        var google = (int)ExternalLoginProvider.Google;
        var facebook = (int)ExternalLoginProvider.Facebook;
        var github = (int)ExternalLoginProvider.Github;

        // Assert
        google.Should().BeOfType(typeof(int));
        facebook.Should().BeOfType(typeof(int));
        github.Should().BeOfType(typeof(int));
    }

    [Fact]
    public void ExternalLoginProvider_ShouldHaveDistinctIntegerValues()
    {
        // Arrange & Act
        var values = new[] { (int)ExternalLoginProvider.Google, (int)ExternalLoginProvider.Facebook, (int)ExternalLoginProvider.Github };

        // Assert
        values.Should().OnlyHaveUniqueItems();
    }
}
