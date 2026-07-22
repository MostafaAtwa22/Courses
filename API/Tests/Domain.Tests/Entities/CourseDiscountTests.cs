using Domain.Entities;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class CourseDiscountTests
{
    [Fact]
    public void CourseDiscount_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var discount = new CourseDiscount();

        // Assert
        discount.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void CourseDiscount_ShouldHaveDefaultZeroPercentage()
    {
        // Arrange & Act
        var discount = new CourseDiscount();

        // Assert
        discount.Percentage.Should().Be(0m);
    }

    [Fact]
    public void CourseDiscount_ShouldHaveDefaultTrueIsActive()
    {
        // Arrange & Act
        var discount = new CourseDiscount();

        // Assert
        discount.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CourseDiscount_ShouldAllowSettingPercentage()
    {
        // Arrange
        var expectedPercentage = 20m;

        // Act
        var discount = new CourseDiscount { Percentage = expectedPercentage };

        // Assert
        discount.Percentage.Should().Be(expectedPercentage);
    }

    [Fact]
    public void CourseDiscount_ShouldAllowSettingStartTime()
    {
        // Arrange
        var expectedStartTime = DateTimeOffset.UtcNow;

        // Act
        var discount = new CourseDiscount { StartTime = expectedStartTime };

        // Assert
        discount.StartTime.Should().Be(expectedStartTime);
    }

    [Fact]
    public void CourseDiscount_ShouldAllowSettingEndTime()
    {
        // Arrange
        var expectedEndTime = DateTimeOffset.UtcNow.AddDays(7);

        // Act
        var discount = new CourseDiscount { EndTime = expectedEndTime };

        // Assert
        discount.EndTime.Should().Be(expectedEndTime);
    }

    [Fact]
    public void CourseDiscount_ShouldAllowSettingIsActive()
    {
        // Arrange & Act
        var discount = new CourseDiscount { IsActive = false };

        // Assert
        discount.IsActive.Should().BeFalse();
    }

    [Fact]
    public void CourseDiscount_ShouldAllowSettingCourseId()
    {
        // Arrange
        var expectedCourseId = Guid.NewGuid();

        // Act
        var discount = new CourseDiscount { CourseId = expectedCourseId };

        // Assert
        discount.CourseId.Should().Be(expectedCourseId);
    }

    [Fact]
    public void CourseDiscount_ShouldAllowSettingCourse()
    {
        // Arrange
        var expectedCourse = new Course();

        // Act
        var discount = new CourseDiscount { Course = expectedCourse };

        // Assert
        discount.Course.Should().Be(expectedCourse);
    }

    [Fact]
    public void CourseDiscount_ShouldAllowValidPercentageRange()
    {
        // Arrange & Act
        var minDiscount = new CourseDiscount { Percentage = 0m };
        var maxDiscount = new CourseDiscount { Percentage = 100m };
        var midDiscount = new CourseDiscount { Percentage = 50m };

        // Assert
        minDiscount.Percentage.Should().Be(0m);
        maxDiscount.Percentage.Should().Be(100m);
        midDiscount.Percentage.Should().Be(50m);
    }

    [Fact]
    public void CourseDiscount_ShouldAllowDecimalPercentage()
    {
        // Arrange
        var expectedPercentage = 25.5m;

        // Act
        var discount = new CourseDiscount { Percentage = expectedPercentage };

        // Assert
        discount.Percentage.Should().Be(expectedPercentage);
    }

    [Fact]
    public void CourseDiscount_ShouldHaveValidTimeRange()
    {
        // Arrange
        var startTime = DateTimeOffset.UtcNow;
        var endTime = startTime.AddDays(7);

        // Act
        var discount = new CourseDiscount
        {
            StartTime = startTime,
            EndTime = endTime
        };

        // Assert
        discount.StartTime.Should().BeBefore(discount.EndTime);
    }
}
