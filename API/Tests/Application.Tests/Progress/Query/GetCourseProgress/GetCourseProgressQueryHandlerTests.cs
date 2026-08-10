using Application.Common.Interfaces;
using Application.DTOs.Progress;
using Application.Features.Progress.Queries.GetCourseProgress;
using FluentAssertions;
using Moq;

namespace Application.Tests.Progress.Query.GetCourseProgress
{
    public class GetCourseProgressQueryHandlerTests
    {
        private readonly Mock<IContentProgressRepository> _progressRepoMock;
        private readonly GetCourseProgressQueryHandler _handler;

        public GetCourseProgressQueryHandlerTests()
        {
            _progressRepoMock = new Mock<IContentProgressRepository>();
            _handler = new GetCourseProgressQueryHandler(_progressRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProgress_WhenEnrolledStudent()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var completedContentIds = new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            var summary = new CourseProgressSummaryDto
            {
                CourseId = courseId,
                CompletedCount = 2,
                TotalCount = 5,
                PercentComplete = 40
            };

            var query = new GetCourseProgressQuery(courseId) { StudentId = studentId };

            _progressRepoMock.Setup(r => r.GetCourseProgressSummaryAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summary);
            _progressRepoMock.Setup(r => r.GetCompletedContentIdsAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(completedContentIds);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.CourseId.Should().Be(courseId);
            result.CompletedCount.Should().Be(2);
            result.TotalCount.Should().Be(5);
            result.PercentComplete.Should().Be(40);
            result.CompletedContentIds.Should().BeEquivalentTo(completedContentIds);
        }

        [Fact]
        public async Task Handle_ShouldReturnZeroProgress_WhenNoContentCompleted()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var summary = new CourseProgressSummaryDto
            {
                CourseId = courseId,
                CompletedCount = 0,
                TotalCount = 5,
                PercentComplete = 0
            };

            var query = new GetCourseProgressQuery(courseId) { StudentId = studentId };

            _progressRepoMock.Setup(r => r.GetCourseProgressSummaryAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summary);
            _progressRepoMock.Setup(r => r.GetCompletedContentIdsAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HashSet<Guid>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.CompletedCount.Should().Be(0);
            result.PercentComplete.Should().Be(0);
            result.CompletedContentIds.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturn100Percent_WhenAllContentCompleted()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var completedContentIds = new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            var summary = new CourseProgressSummaryDto
            {
                CourseId = courseId,
                CompletedCount = 3,
                TotalCount = 3,
                PercentComplete = 100
            };

            var query = new GetCourseProgressQuery(courseId) { StudentId = studentId };

            _progressRepoMock.Setup(r => r.GetCourseProgressSummaryAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(summary);
            _progressRepoMock.Setup(r => r.GetCompletedContentIdsAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(completedContentIds);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.PercentComplete.Should().Be(100);
            result.CompletedCount.Should().Be(result.TotalCount);
        }
    }
}
