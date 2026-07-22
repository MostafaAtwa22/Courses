using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class CourseTests
{
    [Fact]
    public void Course_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Course_ShouldHaveDefaultEmptyTitle()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Title.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldHaveDefaultEmptyDescription()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Description.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldHaveDefaultEmptyPictureUrl()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.PictureUrl.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldHaveDefaultInProgressStatus()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Status.Should().Be(CourseStatus.InProgress);
    }

    [Fact]
    public void Course_ShouldHaveDefaultZeroCost()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Cost.Should().Be(0m);
    }

    [Fact]
    public void Course_ShouldHaveDefaultZeroStudentCount()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.StudentCount.Should().Be(0);
    }

    [Fact]
    public void Course_ShouldHaveDefaultZeroTotalReviews()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.TotalReviews.Should().Be(0);
    }

    [Fact]
    public void Course_ShouldHaveDefaultZeroAverageRate()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.AverageRate.Should().Be(0m);
    }

    [Fact]
    public void Course_ShouldHaveDefaultEnglishLanguage()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Language.Should().Be("English");
    }

    [Fact]
    public void Course_ShouldInitializeWhatYouWillLearnAsEmptyList()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.WhatYouWillLearn.Should().NotBeNull();
        course.WhatYouWillLearn.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldInitializeRequirementsAsEmptyList()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Requirements.Should().NotBeNull();
        course.Requirements.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldHaveDefaultEmptyIntroVideoUrl()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.IntroVideoUrl.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldInitializeReviewsAsEmptyCollection()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Reviews.Should().NotBeNull();
        course.Reviews.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldInitializeEnrollmentsAsEmptyCollection()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Enrollments.Should().NotBeNull();
        course.Enrollments.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldInitializeSectionsAsEmptyCollection()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.Sections.Should().NotBeNull();
        course.Sections.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldInitializeCourseDiscountsAsEmptyCollection()
    {
        // Arrange & Act
        var course = new Course();

        // Assert
        course.CourseDiscounts.Should().NotBeNull();
        course.CourseDiscounts.Should().BeEmpty();
    }

    [Fact]
    public void Course_ShouldAllowSettingTitle()
    {
        // Arrange
        var expectedTitle = "C# Programming";

        // Act
        var course = new Course { Title = expectedTitle };

        // Assert
        course.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void Course_ShouldAllowSettingDescription()
    {
        // Arrange
        var expectedDescription = "Learn C# from scratch";

        // Act
        var course = new Course { Description = expectedDescription };

        // Assert
        course.Description.Should().Be(expectedDescription);
    }

    [Fact]
    public void Course_ShouldAllowSettingCost()
    {
        // Arrange
        var expectedCost = 99.99m;

        // Act
        var course = new Course { Cost = expectedCost };

        // Assert
        course.Cost.Should().Be(expectedCost);
    }

    [Fact]
    public void Course_ShouldAllowSettingStatus()
    {
        // Arrange
        var expectedStatus = CourseStatus.Done;

        // Act
        var course = new Course { Status = expectedStatus };

        // Assert
        course.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public void Course_ShouldAllowSettingLanguage()
    {
        // Arrange
        var expectedLanguage = "Spanish";

        // Act
        var course = new Course { Language = expectedLanguage };

        // Assert
        course.Language.Should().Be(expectedLanguage);
    }

    [Fact]
    public void Course_ShouldAllowSettingWhatYouWillLearn()
    {
        // Arrange
        var expectedLearning = new List<string> { "C# Basics", "OOP", "Async Programming" };

        // Act
        var course = new Course { WhatYouWillLearn = expectedLearning };

        // Assert
        course.WhatYouWillLearn.Should().HaveCount(3);
        course.WhatYouWillLearn.Should().BeEquivalentTo(expectedLearning);
    }

    [Fact]
    public void Course_ShouldAllowSettingRequirements()
    {
        // Arrange
        var expectedRequirements = new List<string> { "Basic programming knowledge", "Computer" };

        // Act
        var course = new Course { Requirements = expectedRequirements };

        // Assert
        course.Requirements.Should().HaveCount(2);
        course.Requirements.Should().BeEquivalentTo(expectedRequirements);
    }

    [Fact]
    public void Course_ShouldAllowSettingCategoryId()
    {
        // Arrange
        var expectedCategoryId = Guid.NewGuid();

        // Act
        var course = new Course { CategoryId = expectedCategoryId };

        // Assert
        course.CategoryId.Should().Be(expectedCategoryId);
    }

    [Fact]
    public void Course_ShouldAllowSettingInstructorId()
    {
        // Arrange
        var expectedInstructorId = Guid.NewGuid();

        // Act
        var course = new Course { InstructorId = expectedInstructorId };

        // Assert
        course.InstructorId.Should().Be(expectedInstructorId);
    }

    [Fact]
    public void Course_ShouldAllowAddingReviews()
    {
        // Arrange
        var course = new Course();
        var review = new Review { Rating = 5 };

        // Act
        course.Reviews.Add(review);

        // Assert
        course.Reviews.Should().HaveCount(1);
        course.Reviews.Should().Contain(review);
    }

    [Fact]
    public void Course_ShouldAllowAddingSections()
    {
        // Arrange
        var course = new Course();
        var section = new Section { Title = "Introduction" };

        // Act
        course.Sections.Add(section);

        // Assert
        course.Sections.Should().HaveCount(1);
        course.Sections.Should().Contain(section);
    }
}
