using Application.Common.Interfaces;
using Application.DTOs.Review;
using Application.Features.Reviews.Queries.GetById;
using FluentAssertions;
using Moq;

namespace Application.Tests.Reviews.Queries;

public class GetReviewByIdQueryHandlerTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly GetReviewByIdQueryHandler _handler;

    public GetReviewByIdQueryHandlerTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _handler = new GetReviewByIdQueryHandler(_reviewRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnReviewDto_WhenReviewExists()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var query = new GetReviewByIdQuery(reviewId);
        var expectedDto = new ReviewResponseDto 
        { 
            Id = reviewId,
            Headline = "Great course!",
            Rating = 5
        };

        _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        _reviewRepositoryMock.Verify(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenReviewDoesNotExist()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var query = new GetReviewByIdQuery(reviewId);

        _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReviewResponseDto?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _reviewRepositoryMock.Verify(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
