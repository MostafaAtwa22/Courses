using Application.Common.Interfaces;
using Application.DTOs.Course;
using Application.Features.Courses.Commands.Create;
using Domain.Enums;
using FluentAssertions;
using Moq;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Tests.Courses.Command
{
    public class CreateCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _repoMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly CreateCourseCommandHandler _handler;

        public CreateCourseCommandHandlerTests()
        {
            _repoMock = new Mock<ICourseRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _handler = new CreateCourseCommandHandler(_repoMock.Object, _fileServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNewGuid_WhenCourseIsCreated()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            fileMock.Setup(f => f.FileName).Returns("test.jpg");

            var createDto = new CourseCreateDto { Title = "Test Course", Description = "Test Desc", Status = CourseStatus.InProgress, Cost = 100, CategoryId = Guid.NewGuid(), PictureUrl = fileMock.Object };
            var command = new CreateCourseCommand(createDto);
            var expectedId = Guid.NewGuid();

            _fileServiceMock.Setup(f => f.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("images/courses/test.jpg");

            _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(f => f.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
