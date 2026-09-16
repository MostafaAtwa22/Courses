using Application.Features.AdminDashboard.Queries.GetEnrollmentStatistics;
using Application.Common.Interfaces;
using Application.DTOs.AdminDashboard;
using FluentAssertions;
using Moq;

namespace Application.Tests.AdminDashboard.Queries.GetEnrollmentStatistics
{
    public class GetEnrollmentStatisticsQueryHandlerTests
    {
        private readonly Mock<IAdminDashboardRepository> _repoMock;
        private readonly GetEnrollmentStatisticsQueryHandler _handler;

        public GetEnrollmentStatisticsQueryHandlerTests()
        {
            _repoMock = new Mock<IAdminDashboardRepository>();
            _handler = new GetEnrollmentStatisticsQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnEnrollmentStatistics_WhenDataExists()
        {
            // Arrange
            var expectedData = new List<EnrollmentStatisticsDto>
            {
                new() { Period = "Jan", EnrollmentCount = 100 },
                new() { Period = "Feb", EnrollmentCount = 150 },
                new() { Period = "Mar", EnrollmentCount = 200 }
            };

            _repoMock
                .Setup(r => r.GetEnrollmentStatisticsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            var query = new GetEnrollmentStatisticsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedData);
            _repoMock.Verify(r => r.GetEnrollmentStatisticsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var expectedData = new List<EnrollmentStatisticsDto>();

            _repoMock
                .Setup(r => r.GetEnrollmentStatisticsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            var query = new GetEnrollmentStatisticsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _repoMock.Verify(r => r.GetEnrollmentStatisticsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturn12Months_WhenRepositoryReturns12Months()
        {
            // Arrange
            var expectedData = new List<EnrollmentStatisticsDto>();
            for (int i = 1; i <= 12; i++)
            {
                expectedData.Add(new EnrollmentStatisticsDto 
                { 
                    Period = "Month" + i, 
                    EnrollmentCount = i * 10 
                });
            }

            _repoMock
                .Setup(r => r.GetEnrollmentStatisticsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            var query = new GetEnrollmentStatisticsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(12);
            _repoMock.Verify(r => r.GetEnrollmentStatisticsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
