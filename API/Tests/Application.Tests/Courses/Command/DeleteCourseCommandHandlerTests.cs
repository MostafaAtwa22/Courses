using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Course;
using Application.Features.Courses.Commands.Delete;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Courses.Command
{
    public class DeleteCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _repoMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly DeleteCourseCommandHandler _handler;

        public DeleteCourseCommandHandlerTests()
        {
            _repoMock = new Mock<ICourseRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _handler = new DeleteCourseCommandHandler(_repoMock.Object, _fileServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallDeleteAsync_WhenCourseExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteCourseCommand(id);
            var course = new Course { Id = id, PictureUrl = "test.jpg" };
            
            _repoMock.Setup(r => r.GetEntityByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            _repoMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _fileServiceMock.Setup(f => f.DeleteAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _fileServiceMock.Verify(f => f.DeleteAsync("test.jpg"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteCourseCommand(id);

            _repoMock.Setup(r => r.GetEntityByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Course?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Entity \"Course\" with key \"{id}\" was not found.");
        }
    }
}
