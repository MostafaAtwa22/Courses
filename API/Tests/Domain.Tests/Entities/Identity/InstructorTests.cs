using Domain.Entities;
using Domain.Entities.Identity;
using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Entities.Identity;

public class InstructorTests
{
    [Fact]
    public void Instructor_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Instructor_ShouldHaveDefaultEmptyBio()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.Bio.Should().BeEmpty();
    }

    [Fact]
    public void Instructor_ShouldHaveDefaultEmptyTitle()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.Title.Should().BeEmpty();
    }

    [Fact]
    public void Instructor_ShouldHaveDefaultEmptyLinkedInProfileUrl()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.LinkedInProfileUrl.Should().BeEmpty();
    }

    [Fact]
    public void Instructor_ShouldHaveDefaultEmptyGitHubProfileUrl()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.GitHubProfileUrl.Should().BeEmpty();
    }

    [Fact]
    public void Instructor_ShouldHaveDefaultEmptyCvUrl()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.CvUrl.Should().BeEmpty();
    }

    [Fact]
    public void Instructor_ShouldHaveDefaultPendingStatus()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.Status.Should().Be(InstructorStatus.Pending);
    }

    [Fact]
    public void Instructor_ShouldHaveDefaultEmptyUserId()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.UserId.Should().BeEmpty();
    }

    [Fact]
    public void Instructor_ShouldInitializeCoursesAsEmptyCollection()
    {
        // Arrange & Act
        var instructor = new Instructor();

        // Assert
        instructor.Courses.Should().NotBeNull();
        instructor.Courses.Should().BeEmpty();
    }

    [Fact]
    public void Instructor_ShouldAllowSettingBio()
    {
        // Arrange
        var expectedBio = "Experienced software developer";

        // Act
        var instructor = new Instructor { Bio = expectedBio };

        // Assert
        instructor.Bio.Should().Be(expectedBio);
    }

    [Fact]
    public void Instructor_ShouldAllowSettingTitle()
    {
        // Arrange
        var expectedTitle = "Senior Software Engineer";

        // Act
        var instructor = new Instructor { Title = expectedTitle };

        // Assert
        instructor.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void Instructor_ShouldAllowSettingLinkedInProfileUrl()
    {
        // Arrange
        var expectedUrl = "https://linkedin.com/in/johndoe";

        // Act
        var instructor = new Instructor { LinkedInProfileUrl = expectedUrl };

        // Assert
        instructor.LinkedInProfileUrl.Should().Be(expectedUrl);
    }

    [Fact]
    public void Instructor_ShouldAllowSettingGitHubProfileUrl()
    {
        // Arrange
        var expectedUrl = "https://github.com/johndoe";

        // Act
        var instructor = new Instructor { GitHubProfileUrl = expectedUrl };

        // Assert
        instructor.GitHubProfileUrl.Should().Be(expectedUrl);
    }

    [Fact]
    public void Instructor_ShouldAllowSettingCvUrl()
    {
        // Arrange
        var expectedUrl = "https://example.com/cv.pdf";

        // Act
        var instructor = new Instructor { CvUrl = expectedUrl };

        // Assert
        instructor.CvUrl.Should().Be(expectedUrl);
    }

    [Fact]
    public void Instructor_ShouldAllowSettingStatus()
    {
        // Arrange
        var expectedStatus = InstructorStatus.Verfied;

        // Act
        var instructor = new Instructor { Status = expectedStatus };

        // Assert
        instructor.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public void Instructor_ShouldAllowSettingUserId()
    {
        // Arrange
        var expectedUserId = "user123";

        // Act
        var instructor = new Instructor { UserId = expectedUserId };

        // Assert
        instructor.UserId.Should().Be(expectedUserId);
    }

    [Fact]
    public void Instructor_ShouldAllowSettingUser()
    {
        // Arrange
        var expectedUser = new ApplicationUser();

        // Act
        var instructor = new Instructor { User = expectedUser };

        // Assert
        instructor.User.Should().Be(expectedUser);
    }

    [Fact]
    public void Instructor_ShouldAllowAddingCourses()
    {
        // Arrange
        var instructor = new Instructor();
        var course = new Course { Title = "C# Basics" };

        // Act
        instructor.Courses.Add(course);

        // Assert
        instructor.Courses.Should().HaveCount(1);
        instructor.Courses.Should().Contain(course);
    }

    [Fact]
    public void Instructor_ShouldAllowDifferentStatuses()
    {
        // Arrange & Act
        var pendingInstructor = new Instructor { Status = InstructorStatus.Pending };
        var verfiedInstructor = new Instructor { Status = InstructorStatus.Verfied };
        var unverfiedInstructor = new Instructor { Status = InstructorStatus.Unverfied };

        // Assert
        pendingInstructor.Status.Should().Be(InstructorStatus.Pending);
        verfiedInstructor.Status.Should().Be(InstructorStatus.Verfied);
        unverfiedInstructor.Status.Should().Be(InstructorStatus.Unverfied);
    }
}
