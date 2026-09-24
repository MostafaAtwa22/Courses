using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Content;
using Application.Features.Contents.Queries.GetByCourse;
using FluentAssertions;
using Moq;

namespace Application.Tests.Contents.Queries
{
    public class GetContentByCourseQueryHandlerTests
    {
        private readonly Mock<IContentRepository> _repoMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly GetContentByCourseQueryHandler _handler;

        public GetContentByCourseQueryHandlerTests()
        {
            _repoMock = new Mock<IContentRepository>();
            _cacheMock = new Mock<IAppCache>();
            _handler = new GetContentByCourseQueryHandler(_repoMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedContent()
        {
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams();
            var expected = new PaginatedResult<ContentResponseDto>
            {
                Items = new List<ContentResponseDto>
                {
                    new ContentResponseDto { Id = Guid.NewGuid(), ContentUrl = "http://example.com/video.mp4", IsPreview = false }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };
            
            _repoMock.Setup(x => x.GetByCourseAsync(courseId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            _cacheMock
                .Setup(cache => cache.GetOrCreateAsync<PaginatedResult<ContentResponseDto>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<PaginatedResult<ContentResponseDto>?>>>(),
                    It.IsAny<CacheOptions>(),
                    It.IsAny<string[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string key, Func<CancellationToken, ValueTask<PaginatedResult<ContentResponseDto>?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                    return factory(ct);
                });

            var result = await _handler.Handle(new GetContentByCourseQuery(courseId, queryParams), CancellationToken.None);

            result.Should().Be(expected);
            result.Items.First().ContentUrl.Should().Be("http://example.com/video.mp4");
        }
    }
}
