using Domain.Entities;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class SectionTests
{
    [Fact]
    public void Section_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var section = new Section();

        // Assert
        section.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Section_ShouldHaveDefaultEmptyTitle()
    {
        // Arrange & Act
        var section = new Section();

        // Assert
        section.Title.Should().BeEmpty();
    }

    [Fact]
    public void Section_ShouldHaveDefaultZeroOrder()
    {
        // Arrange & Act
        var section = new Section();

        // Assert
        section.Order.Should().Be(0);
    }

    [Fact]
    public void Section_ShouldInitializeContentsAsEmptyCollection()
    {
        // Arrange & Act
        var section = new Section();

        // Assert
        section.Contents.Should().NotBeNull();
        section.Contents.Should().BeEmpty();
    }

    [Fact]
    public void Section_ShouldAllowSettingTitle()
    {
        // Arrange
        var expectedTitle = "Introduction";

        // Act
        var section = new Section { Title = expectedTitle };

        // Assert
        section.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void Section_ShouldAllowSettingOrder()
    {
        // Arrange
        var expectedOrder = 1;

        // Act
        var section = new Section { Order = expectedOrder };

        // Assert
        section.Order.Should().Be(expectedOrder);
    }

    [Fact]
    public void Section_ShouldAllowSettingCourseId()
    {
        // Arrange
        var expectedCourseId = Guid.NewGuid();

        // Act
        var section = new Section { CourseId = expectedCourseId };

        // Assert
        section.CourseId.Should().Be(expectedCourseId);
    }

    [Fact]
    public void Section_ShouldAllowAddingContents()
    {
        // Arrange
        var section = new Section();
        var content = new Content { Title = "Video 1" };

        // Act
        section.Contents.Add(content);

        // Assert
        section.Contents.Should().HaveCount(1);
        section.Contents.Should().Contain(content);
    }

    [Fact]
    public void Section_ShouldAllowMultipleContents()
    {
        // Arrange
        var section = new Section();
        var content1 = new Content { Title = "Video 1" };
        var content2 = new Content { Title = "Video 2" };

        // Act
        section.Contents.Add(content1);
        section.Contents.Add(content2);

        // Assert
        section.Contents.Should().HaveCount(2);
        section.Contents.Should().Contain(content1);
        section.Contents.Should().Contain(content2);
    }

    [Fact]
    public void Section_ShouldAllowOrdering()
    {
        // Arrange & Act
        var section1 = new Section { Order = 1 };
        var section2 = new Section { Order = 2 };
        var section3 = new Section { Order = 3 };

        // Assert
        section1.Order.Should().BeLessThan(section2.Order);
        section2.Order.Should().BeLessThan(section3.Order);
    }
}
