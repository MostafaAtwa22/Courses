using Domain.Entities;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class CategoryTests
{
    [Fact]
    public void Category_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Category_ShouldHaveDefaultEmptyName()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Name.Should().BeEmpty();
    }

    [Fact]
    public void Category_ShouldHaveDefaultEmptySlug()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Slug.Should().BeEmpty();
    }

    [Fact]
    public void Category_ShouldInitializeCoursesAsEmptyCollection()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        category.Courses.Should().NotBeNull();
        category.Courses.Should().BeEmpty();
    }

    [Fact]
    public void Category_ShouldAllowSettingName()
    {
        // Arrange
        var expectedName = "Programming";

        // Act
        var category = new Category { Name = expectedName };

        // Assert
        category.Name.Should().Be(expectedName);
    }

    [Fact]
    public void Category_ShouldAllowSettingSlug()
    {
        // Arrange
        var expectedSlug = "programming";

        // Act
        var category = new Category { Slug = expectedSlug };

        // Assert
        category.Slug.Should().Be(expectedSlug);
    }

    [Fact]
    public void Category_ShouldAllowSettingCourses()
    {
        // Arrange
        var expectedCourses = new List<Course>
        {
            new Course { Title = "Course 1" },
            new Course { Title = "Course 2" }
        };

        // Act
        var category = new Category { Courses = expectedCourses };

        // Assert
        category.Courses.Should().HaveCount(2);
        category.Courses.Should().BeEquivalentTo(expectedCourses);
    }

    [Fact]
    public void Category_ShouldAllowAddingCoursesToCollection()
    {
        // Arrange
        var category = new Category();
        var course = new Course { Title = "New Course" };

        // Act
        category.Courses.Add(course);

        // Assert
        category.Courses.Should().HaveCount(1);
        category.Courses.Should().Contain(course);
    }
}
