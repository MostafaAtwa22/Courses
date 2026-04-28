using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Course;
using Application.Features.Courses.Commands.Update;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Courses.Command
{
    public class UpdateCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _repoMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly UpdateCourseCommandHandler _handler;

        public UpdateCourseCommandHandlerTests()
        {
            _repoMock = new Mock<ICourseRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _handler = new UpdateCourseCommandHandler(_repoMock.Object, _fileServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateAsync_WhenCourseExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var updateDto = new CourseUpdateDto { Title = "Updated" };
            var command = new UpdateCourseCommand(id, updateDto);
            var course = new Course { Id = id, Title = "Old" };
            
            _repoMock.Setup(r => r.GetEntityByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(course);

            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<Course>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new UpdateCourseCommand(id, new CourseUpdateDto());

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
