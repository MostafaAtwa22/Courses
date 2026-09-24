using Application.Features.InstructorDashboard.Queries.GetInstructorStatistics;
using Application.Common.Interfaces;
using Application.DTOs.InstructorDashboard;
using FluentAssertions;
using Moq;

namespace Application.Tests.InstructorDashboard.Queries.GetInstructorStatistics
{
    public class GetInstructorStatisticsQueryHandlerTests
    {
        private readonly Mock<IInstructorDashboardRepository> _repoMock;
        private readonly GetInstructorStatisticsQueryHandler _handler;

        public GetInstructorStatisticsQueryHandlerTests()
        {
            _repoMock = new Mock<IInstructorDashboardRepository>();
            _handler = new GetInstructorStatisticsQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnInstructorStatistics_WhenDataExists()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 1240,
                MonthlyEarnings = 12450.00m,
                InstructorRating = 4.92m,
                CoursesCreated = 4
            };

            _repoMock
                .Setup(r => r.GetInstructorStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            var query = new GetInstructorStatisticsQuery(instructorId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedStats);
            _repoMock.Verify(r => r.GetInstructorStatisticsAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnZeroes_WhenInstructorHasNoData()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 0,
                MonthlyEarnings = 0m,
                InstructorRating = 0m,
                CoursesCreated = 0
            };

            _repoMock
                .Setup(r => r.GetInstructorStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            var query = new GetInstructorStatisticsQuery(instructorId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.EnrolledStudents.Should().Be(0);
            result.MonthlyEarnings.Should().Be(0);
            result.InstructorRating.Should().Be(0);
            result.CoursesCreated.Should().Be(0);
            _repoMock.Verify(r => r.GetInstructorStatisticsAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryWithCorrectInstructorId()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 500,
                MonthlyEarnings = 2500.00m,
                InstructorRating = 4.5m,
                CoursesCreated = 3
            };

            _repoMock
                .Setup(r => r.GetInstructorStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            var query = new GetInstructorStatisticsQuery(instructorId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.GetInstructorStatisticsAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassCancellationToken()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 100,
                MonthlyEarnings = 1000.00m,
                InstructorRating = 4.0m,
                CoursesCreated = 1
            };

            _repoMock
                .Setup(r => r.GetInstructorStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            var query = new GetInstructorStatisticsQuery(instructorId);
            var cancellationToken = new CancellationToken();

            // Act
            await _handler.Handle(query, cancellationToken);

            // Assert
            _repoMock.Verify(r => r.GetInstructorStatisticsAsync(instructorId, cancellationToken), Times.Once);
        }
    }
}