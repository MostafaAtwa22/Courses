using Application.Common.Interfaces;
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
        private readonly GetContentByCourseQueryHandler _handler;

        public GetContentByCourseQueryHandlerTests()
        {
            _repoMock = new Mock<IContentRepository>();
            _handler = new GetContentByCourseQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedContent()
        {
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams();
            var expected = new PaginatedResult<ContentResponseDto>
            {
                Items = new List<ContentResponseDto>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            };
            
            _repoMock.Setup(x => x.GetByCourseAsync(courseId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _handler.Handle(new GetContentByCourseQuery(courseId, queryParams), CancellationToken.None);

            result.Should().Be(expected);
        }
    }
}
