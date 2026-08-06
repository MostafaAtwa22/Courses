using Application.Common.Interfaces;
using Application.Features.Progress.Queries.GetMyCoursesProgress;
using FluentAssertions;
using Moq;
using Application.Common.Exceptions;

namespace Application.Tests.Progress.Query.GetMyCoursesProgress
{
    public class GetMyCoursesProgressQueryHandlerTests
    {
        private readonly Mock<IContentProgressRepository> _progressRepoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly GetMyCoursesProgressQueryHandler _handler;

        public GetMyCoursesProgressQueryHandlerTests()
        {
            _progressRepoMock = new Mock<IContentProgressRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new GetMyCoursesProgressQueryHandler(
                _progressRepoMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProgress_WhenStudentHasEnrollments()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();

            var progressSummaries = new List<CourseProgressSummary>
            {
                new CourseProgressSummary
                {
                    CourseId = courseId1,
                    CompletedCount = 2,
                    TotalCount = 5,
                    PercentComplete = 40
                },
                new CourseProgressSummary
                {
                    CourseId = courseId2,
                    CompletedCount = 3,
                    TotalCount = 3,
                    PercentComplete = 100
                }
            };

            var query = new GetMyCoursesProgressQuery();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _progressRepoMock.Setup(r => r.GetMyCoursesProgressAsync(studentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(progressSummaries);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result[0].CourseId.Should().Be(courseId1);
            result[0].PercentComplete.Should().Be(40);
            result[1].CourseId.Should().Be(courseId2);
            result[1].PercentComplete.Should().Be(100);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenUserNotLoggedIn()
        {
            // Arrange
            _currentUserServiceMock.Setup(u => u.UserId).Returns((string?)null);
            var query = new GetMyCoursesProgressQuery();

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserNotStudent()
        {
            // Arrange
            var userId = "user-123";

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid?)null);

            var query = new GetMyCoursesProgressQuery();

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenStudentHasNoEnrollments()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();

            var query = new GetMyCoursesProgressQuery();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _progressRepoMock.Setup(r => r.GetMyCoursesProgressAsync(studentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CourseProgressSummary>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnProgressWithZeroPercent_WhenNoContentCompleted()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var progressSummaries = new List<CourseProgressSummary>
            {
                new CourseProgressSummary
                {
                    CourseId = courseId,
                    CompletedCount = 0,
                    TotalCount = 10,
                    PercentComplete = 0
                }
            };

            var query = new GetMyCoursesProgressQuery();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _progressRepoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _progressRepoMock.Setup(r => r.GetMyCoursesProgressAsync(studentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(progressSummaries);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result[0].PercentComplete.Should().Be(0);
            result[0].CompletedCount.Should().Be(0);
        }
    }
}
