using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Content;
using Application.Features.Contents.Commands.Update;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.Tests.Contents.Commands
{
    public class UpdateContentCommandHandlerTests
    {
        private readonly Mock<IContentRepository> _repoMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<ISectionRepository> _sectionRepoMock;
        private readonly UpdateContentCommandHandler _handler;

        public UpdateContentCommandHandlerTests()
        {
            _repoMock = new Mock<IContentRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _sectionRepoMock = new Mock<ISectionRepository>();
            _handler = new UpdateContentCommandHandler(_repoMock.Object, _fileServiceMock.Object, _sectionRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenContentNotFound()
        {
            _repoMock.Setup(x => x.GetEntityByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Content?)null);

            var act = async () => await _handler.Handle(new UpdateContentCommand(Guid.NewGuid(), new ContentUpdateDto()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenSectionNotFound()
        {
            _repoMock.Setup(x => x.GetEntityByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Content { Id = Guid.NewGuid() });
            
            _sectionRepoMock.Setup(x => x.GetEntityByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Section?)null);

            var act = async () => await _handler.Handle(new UpdateContentCommand(Guid.NewGuid(), new ContentUpdateDto()), CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_ShouldUpdateContent_WithoutFile()
        {
            var contentId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            var content = new Content { Id = contentId, ContentUrl = "old.mp4" };

            _repoMock.Setup(x => x.GetEntityByIdAsync(contentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(content);
            _sectionRepoMock.Setup(x => x.GetEntityByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Section { Id = sectionId });

            var command = new UpdateContentCommand(contentId, new ContentUpdateDto { Title = "Updated", SectionId = sectionId });

            await _handler.Handle(command, CancellationToken.None);

            _fileServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Never);
            _repoMock.Verify(x => x.UpdateAsync(It.Is<Content>(c => c.Title == "Updated" && c.ContentUrl == "old.mp4"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUpdateContent_WithNewFile()
        {
            var contentId = Guid.NewGuid();
            var sectionId = Guid.NewGuid();
            var content = new Content { Id = contentId, ContentUrl = "old.mp4" };
            
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("new.mp4");

            _repoMock.Setup(x => x.GetEntityByIdAsync(contentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(content);
            _sectionRepoMock.Setup(x => x.GetEntityByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Section { Id = sectionId });
            _fileServiceMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("videos/new.mp4");

            var command = new UpdateContentCommand(contentId, new ContentUpdateDto { Title = "Updated", SectionId = sectionId, File = fileMock.Object, Type = ContentType.Video });

            await _handler.Handle(command, CancellationToken.None);

            _fileServiceMock.Verify(x => x.DeleteAsync("old.mp4"), Times.Once);
            _fileServiceMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), "new.mp4", It.IsAny<string>()), Times.Once);
            _repoMock.Verify(x => x.UpdateAsync(It.Is<Content>(c => c.Title == "Updated" && c.ContentUrl == "videos/new.mp4"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
