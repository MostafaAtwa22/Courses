using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Review;
using Application.Features.Reviews.Queries.GetById;
using FluentAssertions;
using Moq;

namespace Application.Tests.Reviews.Queries;

public class GetReviewByIdQueryHandlerTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IAppCache> _cacheMock;
    private readonly GetReviewByIdQueryHandler _handler;

    public GetReviewByIdQueryHandlerTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _cacheMock = new Mock<IAppCache>();
        _handler = new GetReviewByIdQueryHandler(_reviewRepositoryMock.Object, _cacheMock.Object);
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

        _cacheMock
            .Setup(cache => cache.GetOrCreateAsync<ReviewResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, ValueTask<ReviewResponseDto?>>>(),
                It.IsAny<CacheOptions>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns((string key, Func<CancellationToken, ValueTask<ReviewResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                var result = factory(ct);
                return factory(ct);
            });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenReviewDoesNotExist()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var query = new GetReviewByIdQuery(reviewId);

        _reviewRepositoryMock.Setup(x => x.GetByIdAsync(reviewId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReviewResponseDto?)null);

        _cacheMock
            .Setup(cache => cache.GetOrCreateAsync<ReviewResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, ValueTask<ReviewResponseDto?>>>(),
                It.IsAny<CacheOptions>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns((string key, Func<CancellationToken, ValueTask<ReviewResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                var result = factory(ct);
                return factory(ct);
            });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
