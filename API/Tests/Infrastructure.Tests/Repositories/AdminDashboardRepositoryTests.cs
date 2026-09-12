using Application.Common.Interfaces;
using Application.DTOs.AdminDashboard;
using Infrastructure.Repositories;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;
using System.Data;

namespace Infrastructure.Tests.Repositories
{
    public class AdminDashboardRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly AdminDashboardRepository _repository;

        public AdminDashboardRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new AdminDashboardRepository(_factoryMock.Object);
        }

        [Fact]
        public async Task GetEnrollmentStatisticsAsync_ShouldReturnEnrollmentData()
        {
            var expectedData = new List<EnrollmentStatisticsDto>
            {
                new() { Period = "Jan", EnrollmentCount = 100 },
                new() { Period = "Feb", EnrollmentCount = 150 },
                new() { Period = "Mar", EnrollmentCount = 200 }
            };

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<EnrollmentStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedData);

            var result = await _repository.GetEnrollmentStatisticsAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task GetEnrollmentStatisticsAsync_ShouldReturnEmptyList_WhenNoEnrollments()
        {
            var expectedData = new List<EnrollmentStatisticsDto>();

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<EnrollmentStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedData);

            var result = await _repository.GetEnrollmentStatisticsAsync();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetEnrollmentStatisticsAsync_ShouldReturn12Months_WithZeroForMissingMonths()
        {
            var expectedData = new List<EnrollmentStatisticsDto>();
            for (int i = 1; i <= 12; i++)
            {
                expectedData.Add(new EnrollmentStatisticsDto 
                { 
                    Period = "Month" + i, 
                    EnrollmentCount = i * 10 
                });
            }

            _connectionMock
                .SetupDapperAsync(c => c.QueryAsync<EnrollmentStatisticsDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expectedData);

            var result = await _repository.GetEnrollmentStatisticsAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(12);
        }
    }
}
