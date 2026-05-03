using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Contents.Commands.Delete;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Contents.Commands
{
    public class DeleteContentCommandHandlerTests
    {
        private readonly Mock<IContentRepository> _repoMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly DeleteContentCommandHandler _handler;

        public DeleteContentCommandHandlerTests()
        {
            _repoMock = new Mock<IContentRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _handler = new DeleteContentCommandHandler(_repoMock.Object, _fileServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteContentAndFile_WhenExists()
        {
            var id = Guid.NewGuid();
            _repoMock.Setup(x => x.GetEntityByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Content { Id = id, ContentUrl = "videos/test.mp4" });

            await _handler.Handle(new DeleteContentCommand(id), CancellationToken.None);

            _fileServiceMock.Verify(x => x.DeleteAsync("videos/test.mp4"), Times.Once);
            _repoMock.Verify(x => x.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenNotExists()
        {
            _repoMock.Setup(x => x.GetEntityByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Content?)null);

            var act = async () => await _handler.Handle(new DeleteContentCommand(Guid.NewGuid()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
