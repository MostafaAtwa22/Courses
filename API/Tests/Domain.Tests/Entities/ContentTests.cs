using Domain.Entities;
using Domain.Enums;
using FluentAssertions;

namespace Domain.Tests.Entities;

public class ContentTests
{
    [Fact]
    public void Content_ShouldInheritFromBaseEntity()
    {
        // Arrange & Act
        var content = new Content();

        // Assert
        content.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Content_ShouldHaveDefaultEmptyTitle()
    {
        // Arrange & Act
        var content = new Content();

        // Assert
        content.Title.Should().BeEmpty();
    }

    [Fact]
    public void Content_ShouldHaveDefaultContentType()
    {
        // Arrange & Act
        var content = new Content();

        // Assert
        content.Type.Should().Be(default(ContentType));
    }

    [Fact]
    public void Content_ShouldHaveDefaultEmptyContentUrl()
    {
        // Arrange & Act
        var content = new Content();

        // Assert
        content.ContentUrl.Should().BeEmpty();
    }

    [Fact]
    public void Content_ShouldHaveDefaultZeroOrder()
    {
        // Arrange & Act
        var content = new Content();

        // Assert
        content.Order.Should().Be(0);
    }

    [Fact]
    public void Content_ShouldHaveDefaultFalseIsPreview()
    {
        // Arrange & Act
        var content = new Content();

        // Assert
        content.IsPreview.Should().BeFalse();
    }

    [Fact]
    public void Content_ShouldAllowSettingTitle()
    {
        // Arrange
        var expectedTitle = "Introduction Video";

        // Act
        var content = new Content { Title = expectedTitle };

        // Assert
        content.Title.Should().Be(expectedTitle);
    }

    [Fact]
    public void Content_ShouldAllowSettingType()
    {
        // Arrange
        var expectedType = ContentType.Video;

        // Act
        var content = new Content { Type = expectedType };

        // Assert
        content.Type.Should().Be(expectedType);
    }

    [Fact]
    public void Content_ShouldAllowSettingContentUrl()
    {
        // Arrange
        var expectedUrl = "https://example.com/video.mp4";

        // Act
        var content = new Content { ContentUrl = expectedUrl };

        // Assert
        content.ContentUrl.Should().Be(expectedUrl);
    }

    [Fact]
    public void Content_ShouldAllowSettingOrder()
    {
        // Arrange
        var expectedOrder = 5;

        // Act
        var content = new Content { Order = expectedOrder };

        // Assert
        content.Order.Should().Be(expectedOrder);
    }

    [Fact]
    public void Content_ShouldAllowSettingIsPreview()
    {
        // Arrange & Act
        var content = new Content { IsPreview = true };

        // Assert
        content.IsPreview.Should().BeTrue();
    }

    [Fact]
    public void Content_ShouldAllowSettingSectionId()
    {
        // Arrange
        var expectedSectionId = Guid.NewGuid();

        // Act
        var content = new Content { SectionId = expectedSectionId };

        // Assert
        content.SectionId.Should().Be(expectedSectionId);
    }

    [Fact]
    public void Content_ShouldAllowDifferentContentTypes()
    {
        // Arrange & Act
        var videoContent = new Content { Type = ContentType.Video };
        var fileContent = new Content { Type = ContentType.File };

        // Assert
        videoContent.Type.Should().Be(ContentType.Video);
        fileContent.Type.Should().Be(ContentType.File);
    }
}
