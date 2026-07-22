using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Entities.Identity;

public class ApplicationUserTests
{
    [Fact]
    public void ApplicationUser_ShouldInheritFromIdentityUser()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.Should().BeAssignableTo<Microsoft.AspNetCore.Identity.IdentityUser>();
    }

    [Fact]
    public void ApplicationUser_ShouldHaveDefaultEmptyFirstName()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.FirstName.Should().BeEmpty();
    }

    [Fact]
    public void ApplicationUser_ShouldHaveDefaultEmptyLastName()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.LastName.Should().BeEmpty();
    }

    [Fact]
    public void ApplicationUser_ShouldHaveDefaultGender()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.Gender.Should().Be(default(Gender));
    }

    [Fact]
    public void ApplicationUser_ShouldHaveNullProfilePictureUrl()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.ProfilePictureUrl.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_ShouldHaveNullStudentProfile()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.StudentProfile.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_ShouldHaveNullInstructorProfile()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.InstructorProfile.Should().BeNull();
    }

    [Fact]
    public void ApplicationUser_ShouldInitializeRefreshTokensAsEmptyCollection()
    {
        // Arrange & Act
        var user = new ApplicationUser();

        // Assert
        user.RefreshTokens.Should().NotBeNull();
        user.RefreshTokens.Should().BeEmpty();
    }

    [Fact]
    public void ApplicationUser_ShouldAllowSettingFirstName()
    {
        // Arrange
        var expectedFirstName = "John";

        // Act
        var user = new ApplicationUser { FirstName = expectedFirstName };

        // Assert
        user.FirstName.Should().Be(expectedFirstName);
    }

    [Fact]
    public void ApplicationUser_ShouldAllowSettingLastName()
    {
        // Arrange
        var expectedLastName = "Doe";

        // Act
        var user = new ApplicationUser { LastName = expectedLastName };

        // Assert
        user.LastName.Should().Be(expectedLastName);
    }

    [Fact]
    public void ApplicationUser_ShouldAllowSettingGender()
    {
        // Arrange
        var expectedGender = Gender.Male;

        // Act
        var user = new ApplicationUser { Gender = expectedGender };

        // Assert
        user.Gender.Should().Be(expectedGender);
    }

    [Fact]
    public void ApplicationUser_ShouldAllowSettingProfilePictureUrl()
    {
        // Arrange
        var expectedUrl = "https://example.com/profile.jpg";

        // Act
        var user = new ApplicationUser { ProfilePictureUrl = expectedUrl };

        // Assert
        user.ProfilePictureUrl.Should().Be(expectedUrl);
    }

    [Fact]
    public void ApplicationUser_ShouldAllowSettingStudentProfile()
    {
        // Arrange
        var expectedStudentProfile = new Student();

        // Act
        var user = new ApplicationUser { StudentProfile = expectedStudentProfile };

        // Assert
        user.StudentProfile.Should().Be(expectedStudentProfile);
    }

    [Fact]
    public void ApplicationUser_ShouldAllowSettingInstructorProfile()
    {
        // Arrange
        var expectedInstructorProfile = new Instructor();

        // Act
        var user = new ApplicationUser { InstructorProfile = expectedInstructorProfile };

        // Assert
        user.InstructorProfile.Should().Be(expectedInstructorProfile);
    }

    [Fact]
    public void ApplicationUser_ShouldAllowAddingRefreshTokens()
    {
        // Arrange
        var user = new ApplicationUser();
        var refreshToken = new RefreshToken { Token = "token123" };

        // Act
        user.RefreshTokens.Add(refreshToken);

        // Assert
        user.RefreshTokens.Should().HaveCount(1);
        user.RefreshTokens.Should().Contain(refreshToken);
    }

    [Fact]
    public void ApplicationUser_ShouldAllowMultipleRefreshTokens()
    {
        // Arrange
        var user = new ApplicationUser();
        var token1 = new RefreshToken { Token = "token1" };
        var token2 = new RefreshToken { Token = "token2" };

        // Act
        user.RefreshTokens.Add(token1);
        user.RefreshTokens.Add(token2);

        // Assert
        user.RefreshTokens.Should().HaveCount(2);
    }
}
