using Application.Common.Interfaces;
using Application.DTOs.Progress;
using Application.Features.Progress.Queries.GetMyCoursesProgress;
using FluentAssertions;
using Moq;

namespace Application.Tests.Progress.Query.GetMyCoursesProgress
{
    public class GetMyCoursesProgressQueryHandlerTests
    {
        private readonly Mock<IContentProgressRepository> _progressRepoMock;
        private readonly GetMyCoursesProgressQueryHandler _handler;

        public GetMyCoursesProgressQueryHandlerTests()
        {
            _progressRepoMock = new Mock<IContentProgressRepository>();
            _handler = new GetMyCoursesProgressQueryHandler(_progressRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnProgress_WhenStudentHasEnrollments()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();

            var progressSummaries = new List<CourseProgressSummaryDto>
            {
                new CourseProgressSummaryDto
                {
                    CourseId = courseId1,
                    CompletedCount = 2,
                    TotalCount = 5,
                    PercentComplete = 40
                },
                new CourseProgressSummaryDto
                {
                    CourseId = courseId2,
                    CompletedCount = 3,
                    TotalCount = 3,
                    PercentComplete = 100
                }
            };

            var query = new GetMyCoursesProgressQuery { StudentId = studentId };

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
        public async Task Handle_ShouldReturnEmptyList_WhenStudentHasNoEnrollments()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            var query = new GetMyCoursesProgressQuery { StudentId = studentId };

            _progressRepoMock.Setup(r => r.GetMyCoursesProgressAsync(studentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CourseProgressSummaryDto>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnProgressWithZeroPercent_WhenNoContentCompleted()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var progressSummaries = new List<CourseProgressSummaryDto>
            {
                new CourseProgressSummaryDto
                {
                    CourseId = courseId,
                    CompletedCount = 0,
                    TotalCount = 10,
                    PercentComplete = 0
                }
            };

            var query = new GetMyCoursesProgressQuery { StudentId = studentId };

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
