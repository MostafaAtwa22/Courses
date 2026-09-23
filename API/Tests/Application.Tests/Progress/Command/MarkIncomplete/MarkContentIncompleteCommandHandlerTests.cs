using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.Progress;
using Application.Features.Progress.Commands.MarkIncomplete;
using FluentAssertions;
using Moq;

namespace Application.Tests.Progress.Command.MarkIncomplete
{
    public class MarkContentIncompleteCommandHandlerTests
    {
        private readonly Mock<IContentProgressRepository> _progressRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly MarkContentIncompleteCommandHandler _handler;

        public MarkContentIncompleteCommandHandlerTests()
        {
            _progressRepoMock = new Mock<IContentProgressRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _cacheMock = new Mock<IAppCache>();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid().ToString());
            _handler = new MarkContentIncompleteCommandHandler(_progressRepoMock.Object, _currentUserServiceMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldMarkIncomplete_WhenEnrolledStudent()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var contentId = Guid.NewGuid();

            var dto = new MarkProgressRequestDto { ContentId = contentId, CourseId = Guid.NewGuid() };
            var command = new MarkContentIncompleteCommand(dto) { StudentId = studentId };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _progressRepoMock.Verify(r => r.MarkIncompleteAsync(studentId, contentId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldBeIdempotent_WhenAlreadyMarkedIncomplete()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var contentId = Guid.NewGuid();

            var dto = new MarkProgressRequestDto { ContentId = contentId, CourseId = Guid.NewGuid() };
            var command = new MarkContentIncompleteCommand(dto) { StudentId = studentId };

            // Act
            await _handler.Handle(command, CancellationToken.None);
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _progressRepoMock.Verify(r => r.MarkIncompleteAsync(studentId, contentId, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
