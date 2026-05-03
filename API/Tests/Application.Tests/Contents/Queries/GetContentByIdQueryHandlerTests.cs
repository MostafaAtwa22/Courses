using Application.Common.Interfaces;
using Application.DTOs.Content;
using Application.Features.Contents.Queries.GetById;
using FluentAssertions;
using Moq;

namespace Application.Tests.Contents.Queries
{
    public class GetContentByIdQueryHandlerTests
    {
        private readonly Mock<IContentRepository> _repoMock;
        private readonly GetContentByIdQueryHandler _handler;

        public GetContentByIdQueryHandlerTests()
        {
            _repoMock = new Mock<IContentRepository>();
            _handler = new GetContentByIdQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnContent_WhenExists()
        {
            var id = Guid.NewGuid();
            var expected = new ContentResponseDto { Id = id };
            _repoMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _handler.Handle(new GetContentByIdQuery(id), CancellationToken.None);

            result.Should().Be(expected);
        }
    }
}
