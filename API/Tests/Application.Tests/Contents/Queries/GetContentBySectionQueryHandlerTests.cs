using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Content;
using Application.Features.Contents.Queries.GetBySection;
using FluentAssertions;
using Moq;

namespace Application.Tests.Contents.Queries
{
    public class GetContentBySectionQueryHandlerTests
    {
        private readonly Mock<IContentRepository> _repoMock;
        private readonly GetContentBySectionQueryHandler _handler;

        public GetContentBySectionQueryHandlerTests()
        {
            _repoMock = new Mock<IContentRepository>();
            _handler = new GetContentBySectionQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedContent()
        {
            var sectionId = Guid.NewGuid();
            var queryParams = new QueryParams();
            var expected = new PaginatedResult<ContentResponseDto>
            {
                Items = new List<ContentResponseDto>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            };
            
            _repoMock.Setup(x => x.GetBySectionAsync(sectionId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _handler.Handle(new GetContentBySectionQuery(sectionId, queryParams), CancellationToken.None);

            result.Should().Be(expected);
        }
    }
}
