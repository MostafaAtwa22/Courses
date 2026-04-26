using Application.Common.Interfaces;
using Application.DTOs.Course;
using Application.Features.Courses.Commands.Create;
using Domain.Enums;
using FluentAssertions;
using Moq;

namespace Application.Tests.Courses.Command
{
    public class CreateCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _repoMock;
        private readonly CreateCourseCommandHandler _handler;

        public CreateCourseCommandHandlerTests()
        {
            _repoMock = new Mock<ICourseRepository>();
            _handler = new CreateCourseCommandHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNewGuid_WhenCourseIsCreated()
        {
            // Arrange
            var createDto = new CourseCreateDto { Title = "Test Course", Description = "Test Desc", Status = CourseStatus.InProgress, Cost = 100, CategoryId = Guid.NewGuid() };
            var command = new CreateCourseCommand(createDto);
            var expectedId = Guid.NewGuid();

            _repoMock.Setup(repo => repo.CreateAsync(It.IsAny<CourseCreateDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<CourseCreateDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
