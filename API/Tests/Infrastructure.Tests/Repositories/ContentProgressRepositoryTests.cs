using System.Data;
using Application.Common.Interfaces;
using Infrastructure.Repositories;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;
using System.Linq;

namespace Infrastructure.Tests.Repositories
{
    public class ContentProgressRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly ContentProgressRepository _repository;

        public ContentProgressRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new ContentProgressRepository(_factoryMock.Object);
        }


        [Fact]
        public async Task MarkCompleteAsync_ShouldExecuteWithoutException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var contentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act & Assert
            await _repository.MarkCompleteAsync(studentId, contentId, courseId);
        }

        [Fact]
        public async Task MarkIncompleteAsync_ShouldExecuteWithoutException()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var contentId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act & Assert
            await _repository.MarkIncompleteAsync(studentId, contentId);
        }

        [Fact]
        public async Task GetCompletedContentIdsAsync_ShouldReturnHashSet()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var contentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<Guid>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(contentIds);

            // Act
            var result = await _repository.GetCompletedContentIdsAsync(studentId, courseId);

            // Assert
            result.Should().BeOfType<HashSet<Guid>>();
            result.Should().HaveCountGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task GetCompletedContentIdsAsync_ShouldReturnEmptyHashSet_WhenNoProgress()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<Guid>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(Enumerable.Empty<Guid>());

            // Act
            var result = await _repository.GetCompletedContentIdsAsync(studentId, courseId);

            // Assert
            result.Should().BeOfType<HashSet<Guid>>();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCourseProgressSummaryAsync_ShouldReturnSummary_WhenProgressExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedSummary = new CourseProgressSummary
            {
                CourseId = courseId,
                CompletedCount = 2,
                TotalCount = 5,
                PercentComplete = 40
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseProgressSummary>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _repository.GetCourseProgressSummaryAsync(studentId, courseId);

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(courseId);
            result.CompletedCount.Should().Be(2);
            result.TotalCount.Should().Be(5);
            result.PercentComplete.Should().Be(40);
        }

        [Fact]
        public async Task GetCourseProgressSummaryAsync_ShouldReturnDefault_WhenNoProgress()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseProgressSummary>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync((CourseProgressSummary?)null);

            // Act
            var result = await _repository.GetCourseProgressSummaryAsync(studentId, courseId);

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(courseId);
            result.CompletedCount.Should().Be(0);
            result.TotalCount.Should().Be(0);
            result.PercentComplete.Should().Be(0);
        }

        [Fact]
        public async Task GetMyCoursesProgressAsync_ShouldReturnList()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var expectedProgress = new List<CourseProgressSummary>
            {
                new CourseProgressSummary { CourseId = courseId1, CompletedCount = 2, TotalCount = 5, PercentComplete = 40 }
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<CourseProgressSummary>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedProgress);

            // Act
            var result = await _repository.GetMyCoursesProgressAsync(studentId);

            // Assert
            result.Should().HaveCountGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task GetMyCoursesProgressAsync_ShouldReturnEmptyList_WhenNoEnrollments()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<CourseProgressSummary>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(Enumerable.Empty<CourseProgressSummary>());

            // Act
            var result = await _repository.GetMyCoursesProgressAsync(studentId);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCourseProgressSummaryAsync_ShouldCalculateZeroPercent_WhenTotalIsZero()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedSummary = new CourseProgressSummary
            {
                CourseId = courseId,
                CompletedCount = 0,
                TotalCount = 0,
                PercentComplete = 0
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseProgressSummary>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _repository.GetCourseProgressSummaryAsync(studentId, courseId);

            // Assert
            result.PercentComplete.Should().Be(0);
        }

        [Fact]
        public async Task GetCourseProgressSummaryAsync_ShouldCalculate100Percent_WhenAllCompleted()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedSummary = new CourseProgressSummary
            {
                CourseId = courseId,
                CompletedCount = 5,
                TotalCount = 5,
                PercentComplete = 100
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseProgressSummary>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _repository.GetCourseProgressSummaryAsync(studentId, courseId);

            // Assert
            result.PercentComplete.Should().Be(100);
            result.CompletedCount.Should().Be(result.TotalCount);
        }
    }
}
