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
        public async Task Handle_ShouldReturnContentList()
        {
            var sectionId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expected = new List<ContentResponseDto>
            {
                new ContentResponseDto { Id = Guid.NewGuid(), ContentUrl = "http://example.com/video.mp4", IsPreview = false }
            };
            
            _repoMock.Setup(x => x.GetBySectionAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _handler.Handle(new GetContentBySectionQuery(sectionId, courseId), CancellationToken.None);

            result.Should().BeEquivalentTo(expected);
            result.First().ContentUrl.Should().Be("http://example.com/video.mp4");
        }
    }
}
