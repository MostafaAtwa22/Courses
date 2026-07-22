using Application.Common.Interfaces;
using Application.Features.Courses.Queries.GetSuggestions;
using FluentAssertions;
using Moq;

namespace Application.Tests.Courses.Queries;

public class GetCourseSuggestionsQueryHandlerTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly GetCourseSuggestionsQueryHandler _handler;

    public GetCourseSuggestionsQueryHandlerTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _handler = new GetCourseSuggestionsQueryHandler(_courseRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuggestions_WhenTermIsProvided()
    {
        // Arrange
        var term = "C#";
        var query = new GetCourseSuggestionsQuery(term);
        var expectedSuggestions = new[] { "C# Basics", "C# Advanced", "C# for Beginners" };

        _courseRepositoryMock.Setup(x => x.GetSuggestionsAsync(term, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedSuggestions);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(expectedSuggestions);
        _courseRepositoryMock.Verify(x => x.GetSuggestionsAsync(term, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenTermIsNullOrWhiteSpace()
    {
        // Arrange
        var query = new GetCourseSuggestionsQuery("   ");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _courseRepositoryMock.Verify(x => x.GetSuggestionsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmpty_WhenTermIsEmpty()
    {
        // Arrange
        var query = new GetCourseSuggestionsQuery(string.Empty);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _courseRepositoryMock.Verify(x => x.GetSuggestionsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
