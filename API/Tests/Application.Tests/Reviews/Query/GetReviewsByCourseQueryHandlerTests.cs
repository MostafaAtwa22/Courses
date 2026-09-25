using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Review;
using Application.Features.Reviews.Queries.GetByCourse;
using FluentAssertions;
using Moq;

namespace Application.Tests.Reviews.Query
{
    public class GetReviewsByCourseQueryHandlerTests
    {
        private readonly Mock<IReviewRepository> _repoMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly GetReviewsByCourseQueryHandler _handler;

        public GetReviewsByCourseQueryHandlerTests()
        {
            _repoMock = new Mock<IReviewRepository>();
            _cacheMock = new Mock<IAppCache>();
            _handler = new GetReviewsByCourseQueryHandler(_repoMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedResult_WhenCalled()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams { PageNumber = 1, PageSize = 10 };
            var expectedResult = new PaginatedResult<ReviewResponseDto>
            {
                Items = new List<ReviewResponseDto> { new ReviewResponseDto { Headline = "Test" } },
                TotalCount = 1
            };

            _repoMock.Setup(r => r.GetByCourseAsync(courseId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            _cacheMock
                .Setup(cache => cache.GetOrCreateAsync<PaginatedResult<ReviewResponseDto>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<PaginatedResult<ReviewResponseDto>?>>>(),
                    It.IsAny<CacheOptions>(),
                    It.IsAny<string[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string key, Func<CancellationToken, ValueTask<PaginatedResult<ReviewResponseDto>?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                    return factory(ct);
                });

            var query = new GetReviewsByCourseQuery(courseId, queryParams);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _repoMock.Verify(r => r.GetByCourseAsync(courseId, queryParams, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
