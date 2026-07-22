using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Enums;

public class ContentTypeTests
{
    [Fact]
    public void ContentType_ShouldHaveVideoValue()
    {
        // Arrange & Act
        var contentType = ContentType.Video;

        // Assert
        contentType.Should().Be(ContentType.Video);
    }

    [Fact]
    public void ContentType_ShouldHaveFileValue()
    {
        // Arrange & Act
        var contentType = ContentType.File;

        // Assert
        contentType.Should().Be(ContentType.File);
    }

    [Fact]
    public void ContentType_ShouldHaveTwoValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<ContentType>();

        // Assert
        values.Should().HaveCount(2);
    }

    [Fact]
    public void ContentType_ShouldBeComparable()
    {
        // Arrange & Act
        var video = ContentType.Video;
        var file = ContentType.File;

        // Assert
        video.Should().NotBe(file);
    }

    [Fact]
    public void ContentType_ShouldBeAssignableToInt()
    {
        // Arrange & Act
        var video = (int)ContentType.Video;
        var file = (int)ContentType.File;

        // Assert
        video.Should().BeOfType(typeof(int));
        file.Should().BeOfType(typeof(int));
        video.Should().NotBe(file);
    }
}
