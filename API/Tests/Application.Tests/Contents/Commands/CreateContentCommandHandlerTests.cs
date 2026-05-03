using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Content;
using Application.Features.Contents.Commands.Create;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.Tests.Contents.Commands
{
    public class CreateContentCommandHandlerTests
    {
        private readonly Mock<IContentRepository> _repoMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<ISectionRepository> _sectionRepoMock;
        private readonly CreateContentCommandHandler _handler;

        public CreateContentCommandHandlerTests()
        {
            _repoMock = new Mock<IContentRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _sectionRepoMock = new Mock<ISectionRepository>();
            _handler = new CreateContentCommandHandler(_repoMock.Object, _fileServiceMock.Object, _sectionRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateContent_WhenSectionExists()
        {
            var sectionId = Guid.NewGuid();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.mp4");
            
            var dto = new ContentCreateDto
            {
                Title = "Test Video",
                Type = ContentType.Video,
                SectionId = sectionId,
                File = fileMock.Object
            };
            var command = new CreateContentCommand(dto);

            _sectionRepoMock.Setup(x => x.GetEntityByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Section { Id = sectionId });

            _fileServiceMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("videos/test.mp4");

            var expectedId = Guid.NewGuid();
            _repoMock.Setup(x => x.CreateAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
            _fileServiceMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), "test.mp4", It.IsAny<string>()), Times.Once);
            _repoMock.Verify(x => x.CreateAsync(It.Is<Content>(c => c.Title == "Test Video" && c.ContentUrl == "videos/test.mp4"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenSectionDoesNotExist()
        {
            var dto = new ContentCreateDto { SectionId = Guid.NewGuid() };
            var command = new CreateContentCommand(dto);

            _sectionRepoMock.Setup(x => x.GetEntityByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Section?)null);

            var act = async () => await _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
            _repoMock.Verify(x => x.CreateAsync(It.IsAny<Content>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
