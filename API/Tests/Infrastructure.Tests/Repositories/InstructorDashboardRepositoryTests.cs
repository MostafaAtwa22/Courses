using Application.Common.Interfaces;
using Application.DTOs.InstructorDashboard;
using Infrastructure.Repositories;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;
using System.Data;

namespace Infrastructure.Tests.Repositories
{
    public class InstructorDashboardRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly InstructorDashboardRepository _repository;

        public InstructorDashboardRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new InstructorDashboardRepository(_factoryMock.Object);
        }

        [Fact]
        public async Task GetInstructorStatisticsAsync_ShouldReturnStatistics_WhenDataExists()
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

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _repository.GetInstructorStatisticsAsync(instructorId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedStats);
        }

        [Fact]
        public async Task GetInstructorStatisticsAsync_ShouldReturnZeroes_WhenInstructorHasNoData()
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

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _repository.GetInstructorStatisticsAsync(instructorId);

            // Assert
            result.Should().NotBeNull();
            result.EnrolledStudents.Should().Be(0);
            result.MonthlyEarnings.Should().Be(0);
            result.InstructorRating.Should().Be(0);
            result.CoursesCreated.Should().Be(0);
        }

        [Fact]
        public async Task GetInstructorStatisticsAsync_ShouldPassCancellationToken()
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

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedStats);

            var cancellationToken = new CancellationToken();

            // Act
            await _repository.GetInstructorStatisticsAsync(instructorId, cancellationToken);

            // Assert
            _factoryMock.Verify(f => f.CreateConnectionAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetInstructorStatisticsAsync_ShouldHandleDecimalValues()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 999,
                MonthlyEarnings = 9999.99m,
                InstructorRating = 4.95m,
                CoursesCreated = 9
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _repository.GetInstructorStatisticsAsync(instructorId);

            // Assert
            result.Should().NotBeNull();
            result.MonthlyEarnings.Should().Be(9999.99m);
            result.InstructorRating.Should().Be(4.95m);
        }

        [Fact]
        public async Task GetInstructorEnrollmentStatisticsAsync_ShouldReturnEnrollmentData_WhenDataExists()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>
            {
                new() { Period = "Jan", EnrollmentCount = 10 },
                new() { Period = "Feb", EnrollmentCount = 15 },
                new() { Period = "Mar", EnrollmentCount = 20 }
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<InstructorEnrollmentStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _repository.GetInstructorEnrollmentStatisticsAsync(instructorId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetInstructorEnrollmentStatisticsAsync_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>();

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<InstructorEnrollmentStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _repository.GetInstructorEnrollmentStatisticsAsync(instructorId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetInstructorEnrollmentStatisticsAsync_ShouldCallQueryAsync()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>
            {
                new() { Period = "Jan", EnrollmentCount = 5 }
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<InstructorEnrollmentStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedData);

            // Act
            await _repository.GetInstructorEnrollmentStatisticsAsync(instructorId);

            // Assert
            _factoryMock.Verify(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInstructorEnrollmentStatisticsAsync_ShouldPassCancellationToken()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>
            {
                new() { Period = "Jan", EnrollmentCount = 1 }
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<InstructorEnrollmentStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedData);

            var cancellationToken = new CancellationToken();

            // Act
            await _repository.GetInstructorEnrollmentStatisticsAsync(instructorId, cancellationToken);

            // Assert
            _factoryMock.Verify(f => f.CreateConnectionAsync(cancellationToken), Times.Once);
        }
    }
}