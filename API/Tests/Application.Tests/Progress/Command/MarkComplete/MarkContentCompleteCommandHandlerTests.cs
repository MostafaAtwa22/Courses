using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Progress;
using Application.Features.Progress.Commands.MarkComplete;
using FluentAssertions;
using Moq;

namespace Application.Tests.Progress.Command.MarkComplete
{
    public class MarkContentCompleteCommandHandlerTests
    {
        private readonly Mock<IContentProgressRepository> _progressRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly MarkContentCompleteCommandHandler _handler;

        public MarkContentCompleteCommandHandlerTests()
        {
            _progressRepoMock = new Mock<IContentProgressRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _cacheMock = new Mock<IAppCache>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid().ToString());
            _handler = new MarkContentCompleteCommandHandler(_progressRepoMock.Object, _currentUserServiceMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldMarkComplete_WhenEnrolledStudent()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var contentId = Guid.NewGuid();

            var dto = new MarkProgressRequestDto { ContentId = contentId, CourseId = courseId };
            var command = new MarkContentCompleteCommand(dto) { StudentId = studentId };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _progressRepoMock.Verify(r => r.MarkCompleteAsync(studentId, contentId, courseId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldBeIdempotent_WhenAlreadyMarkedComplete()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var contentId = Guid.NewGuid();

            var dto = new MarkProgressRequestDto { ContentId = contentId, CourseId = courseId };
            var command = new MarkContentCompleteCommand(dto) { StudentId = studentId };

            // Act
            await _handler.Handle(command, CancellationToken.None);
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _progressRepoMock.Verify(r => r.MarkCompleteAsync(studentId, contentId, courseId, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
