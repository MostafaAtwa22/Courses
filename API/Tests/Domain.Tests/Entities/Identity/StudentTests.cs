using Domain.Entities;
using Domain.Entities.Identity;
using FluentAssertions;

namespace Domain.Tests.Entities.Identity;

public class StudentTests
{
    [Fact]
    public void Student_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        student.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Student_ShouldHaveDefaultZeroCoins()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        student.Coins.Should().Be(0);
    }

    [Fact]
    public void Student_ShouldHaveDefaultEmptyUserId()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        student.UserId.Should().BeEmpty();
    }

    [Fact]
    public void Student_ShouldInitializeEnrollmentsAsEmptyCollection()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        student.Enrollments.Should().NotBeNull();
        student.Enrollments.Should().BeEmpty();
    }

    [Fact]
    public void Student_ShouldInitializeReviewsAsEmptyCollection()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        student.Reviews.Should().NotBeNull();
        student.Reviews.Should().BeEmpty();
    }

    [Fact]
    public void Student_ShouldAllowSettingCoins()
    {
        // Arrange
        var expectedCoins = 100;

        // Act
        var student = new Student { Coins = expectedCoins };

        // Assert
        student.Coins.Should().Be(expectedCoins);
    }

    [Fact]
    public void Student_ShouldAllowSettingUserId()
    {
        // Arrange
        var expectedUserId = "user123";

        // Act
        var student = new Student { UserId = expectedUserId };

        // Assert
        student.UserId.Should().Be(expectedUserId);
    }

    [Fact]
    public void Student_ShouldAllowSettingUser()
    {
        // Arrange
        var expectedUser = new ApplicationUser();

        // Act
        var student = new Student { User = expectedUser };

        // Assert
        student.User.Should().Be(expectedUser);
    }

    [Fact]
    public void Student_ShouldAllowAddingEnrollments()
    {
        // Arrange
        var student = new Student();
        var enrollment = new Enrollment();

        // Act
        student.Enrollments.Add(enrollment);

        // Assert
        student.Enrollments.Should().HaveCount(1);
        student.Enrollments.Should().Contain(enrollment);
    }

    [Fact]
    public void Student_ShouldAllowAddingReviews()
    {
        // Arrange
        var student = new Student();
        var review = new Review { Rating = 5 };

        // Act
        student.Reviews.Add(review);

        // Assert
        student.Reviews.Should().HaveCount(1);
        student.Reviews.Should().Contain(review);
    }

    [Fact]
    public void Student_ShouldAllowIncrementingCoins()
    {
        // Arrange
        var student = new Student { Coins = 50 };

        // Act
        student.Coins += 25;

        // Assert
        student.Coins.Should().Be(75);
    }

    [Fact]
    public void Student_ShouldAllowDecrementingCoins()
    {
        // Arrange
        var student = new Student { Coins = 100 };

        // Act
        student.Coins -= 30;

        // Assert
        student.Coins.Should().Be(70);
    }

    [Fact]
    public void Student_ShouldAllowMultipleEnrollments()
    {
        // Arrange
        var student = new Student();
        var enrollment1 = new Enrollment();
        var enrollment2 = new Enrollment();

        // Act
        student.Enrollments.Add(enrollment1);
        student.Enrollments.Add(enrollment2);

        // Assert
        student.Enrollments.Should().HaveCount(2);
    }

    [Fact]
    public void Student_ShouldAllowMultipleReviews()
    {
        // Arrange
        var student = new Student();
        var review1 = new Review { Rating = 4 };
        var review2 = new Review { Rating = 5 };

        // Act
        student.Reviews.Add(review1);
        student.Reviews.Add(review2);

        // Assert
        student.Reviews.Should().HaveCount(2);
    }
}
