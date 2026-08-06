using Application.Common.Interfaces;
using Application.DTOs.Progress;
using Application.Features.Progress.Commands.MarkComplete;
using FluentAssertions;
using Moq;
using Application.Common.Exceptions;

namespace Application.Tests.Progress.Command.MarkComplete
{
    public class MarkContentCompleteCommandHandlerTests
    {
        private readonly Mock<IContentProgressRepository> _progressRepoMock;
        private readonly Mock<IEnrollmentRepository> _enrollmentRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly MarkContentCompleteCommandHandler _handler;

        public MarkContentCompleteCommandHandlerTests()
        {
            _progressRepoMock = new Mock<IContentProgressRepository>();
            _enrollmentRepoMock = new Mock<IEnrollmentRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new MarkContentCompleteCommandHandler(
                _progressRepoMock.Object,
                _enrollmentRepoMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldMarkComplete_WhenEnrolledStudent()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var contentId = Guid.NewGuid();

            var dto = new MarkProgressRequestDto { ContentId = contentId, CourseId = courseId };
            var command = new MarkContentCompleteCommand(dto);

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _enrollmentRepoMock.Setup(r => r.IsEnrolledByUserIdAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _enrollmentRepoMock.Setup(r => r.GetCourseIdByContentIdAsync(contentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _progressRepoMock.Verify(r => r.MarkCompleteAsync(studentId, contentId, courseId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenUserNotLoggedIn()
        {
            // Arrange
            _currentUserServiceMock.Setup(u => u.UserId).Returns((string?)null);
            var command = new MarkContentCompleteCommand(new MarkProgressRequestDto());

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserNotStudent()
        {
            // Arrange
            var userId = "user-123";
            var courseId = Guid.NewGuid();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid?)null);

            var command = new MarkContentCompleteCommand(new MarkProgressRequestDto { CourseId = courseId });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenStudentNotEnrolled()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _enrollmentRepoMock.Setup(r => r.IsEnrolledByUserIdAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new MarkContentCompleteCommand(new MarkProgressRequestDto { CourseId = courseId });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowBadRequest_WhenContentNotInCourse()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var contentId = Guid.NewGuid();
            var differentCourseId = Guid.NewGuid();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _enrollmentRepoMock.Setup(r => r.IsEnrolledByUserIdAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _enrollmentRepoMock.Setup(r => r.GetCourseIdByContentIdAsync(contentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(differentCourseId);

            var command = new MarkContentCompleteCommand(new MarkProgressRequestDto { ContentId = contentId, CourseId = courseId });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task Handle_ShouldBeIdempotent_WhenAlreadyMarkedComplete()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var contentId = Guid.NewGuid();

            var dto = new MarkProgressRequestDto { ContentId = contentId, CourseId = courseId };
            var command = new MarkContentCompleteCommand(dto);

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _enrollmentRepoMock.Setup(r => r.IsEnrolledByUserIdAsync(userId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _enrollmentRepoMock.Setup(r => r.GetCourseIdByContentIdAsync(contentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(courseId);

            // Act
            await _handler.Handle(command, CancellationToken.None);
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _progressRepoMock.Verify(r => r.MarkCompleteAsync(studentId, contentId, courseId, It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
