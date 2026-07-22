using Domain.Entities;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class ReviewTests
{
    [Fact]
    public void Review_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var review = new Review();

        // Assert
        review.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Review_ShouldHaveDefaultEmptyHeadline()
    {
        // Arrange & Act
        var review = new Review();

        // Assert
        review.Headline.Should().BeEmpty();
    }

    [Fact]
    public void Review_ShouldHaveDefaultEmptyComment()
    {
        // Arrange & Act
        var review = new Review();

        // Assert
        review.Comment.Should().BeEmpty();
    }

    [Fact]
    public void Review_ShouldHaveDefaultZeroRating()
    {
        // Arrange & Act
        var review = new Review();

        // Assert
        review.Rating.Should().Be(0m);
    }

    [Fact]
    public void Review_ShouldAllowSettingHeadline()
    {
        // Arrange
        var expectedHeadline = "Great course!";

        // Act
        var review = new Review { Headline = expectedHeadline };

        // Assert
        review.Headline.Should().Be(expectedHeadline);
    }

    [Fact]
    public void Review_ShouldAllowSettingComment()
    {
        // Arrange
        var expectedComment = "This course helped me learn C# quickly.";

        // Act
        var review = new Review { Comment = expectedComment };

        // Assert
        review.Comment.Should().Be(expectedComment);
    }

    [Fact]
    public void Review_ShouldAllowSettingRating()
    {
        // Arrange
        var expectedRating = 4.5m;

        // Act
        var review = new Review { Rating = expectedRating };

        // Assert
        review.Rating.Should().Be(expectedRating);
    }

    [Fact]
    public void Review_ShouldAllowSettingCourseId()
    {
        // Arrange
        var expectedCourseId = Guid.NewGuid();

        // Act
        var review = new Review { CourseId = expectedCourseId };

        // Assert
        review.CourseId.Should().Be(expectedCourseId);
    }

    [Fact]
    public void Review_ShouldAllowSettingStudentId()
    {
        // Arrange
        var expectedStudentId = Guid.NewGuid();

        // Act
        var review = new Review { StudentId = expectedStudentId };

        // Assert
        review.StudentId.Should().Be(expectedStudentId);
    }

    [Fact]
    public void Review_ShouldAllowFullRatingRange()
    {
        // Arrange & Act
        var minRating = new Review { Rating = 0m };
        var maxRating = new Review { Rating = 5m };
        var midRating = new Review { Rating = 2.5m };

        // Assert
        minRating.Rating.Should().Be(0m);
        maxRating.Rating.Should().Be(5m);
        midRating.Rating.Should().Be(2.5m);
    }

    [Fact]
    public void Review_ShouldAllowDecimalRating()
    {
        // Arrange
        var expectedRating = 3.7m;

        // Act
        var review = new Review { Rating = expectedRating };

        // Assert
        review.Rating.Should().Be(expectedRating);
    }
}
