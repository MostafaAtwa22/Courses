using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Enums;

public class CourseStatusTests
{
    [Fact]
    public void CourseStatus_ShouldHaveInProgressValue()
    {
        // Arrange & Act
        var status = CourseStatus.InProgress;

        // Assert
        status.Should().Be(CourseStatus.InProgress);
    }

    [Fact]
    public void CourseStatus_ShouldHaveDoneValue()
    {
        // Arrange & Act
        var status = CourseStatus.Done;

        // Assert
        status.Should().Be(CourseStatus.Done);
    }

    [Fact]
    public void CourseStatus_ShouldHaveTwoValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<CourseStatus>();

        // Assert
        values.Should().HaveCount(2);
    }

    [Fact]
    public void CourseStatus_ShouldBeComparable()
    {
        // Arrange & Act
        var inProgress = CourseStatus.InProgress;
        var done = CourseStatus.Done;

        // Assert
        inProgress.Should().NotBe(done);
    }

    [Fact]
    public void CourseStatus_ShouldBeAssignableToInt()
    {
        // Arrange & Act
        var inProgress = (int)CourseStatus.InProgress;
        var done = (int)CourseStatus.Done;

        // Assert
        inProgress.Should().BeOfType(typeof(int));
        done.Should().BeOfType(typeof(int));
        inProgress.Should().NotBe(done);
    }
}
