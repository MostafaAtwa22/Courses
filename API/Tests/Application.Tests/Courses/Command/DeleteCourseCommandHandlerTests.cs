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
        private readonly DeleteCourseCommandHandler _handler;

        public DeleteCourseCommandHandlerTests()
        {
            _repoMock = new Mock<ICourseRepository>();
            _handler = new DeleteCourseCommandHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCallDeleteAsync_WhenCourseExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteCourseCommand(id);
            
            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CoursesResponseDto());

            _repoMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCourseDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var command = new DeleteCourseCommand(id);

            _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((CoursesResponseDto?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"Entity \"Course\" ({id}) was not found.");
        }
    }
}
