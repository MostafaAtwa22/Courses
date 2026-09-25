using Application.Features.InstructorDashboard.Queries.GetInstructorEnrollmentStatistics;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.DTOs.InstructorDashboard;
using FluentAssertions;
using Moq;

namespace Application.Tests.InstructorDashboard.Queries.GetInstructorEnrollmentStatistics
{
    public class GetInstructorEnrollmentStatisticsQueryHandlerTests
    {
        private readonly Mock<IInstructorDashboardRepository> _repoMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly GetInstructorEnrollmentStatisticsQueryHandler _handler;

        public GetInstructorEnrollmentStatisticsQueryHandlerTests()
        {
            _repoMock = new Mock<IInstructorDashboardRepository>();
            _cacheMock = new Mock<IAppCache>();
            _handler = new GetInstructorEnrollmentStatisticsQueryHandler(_repoMock.Object, _cacheMock.Object);

            // Setup default cache behavior to call the factory function
            _cacheMock
                .Setup(cache => cache.GetOrCreateAsync<IEnumerable<InstructorEnrollmentStatisticsDto>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<IEnumerable<InstructorEnrollmentStatisticsDto>?>>>(),
                    It.IsAny<CacheOptions>(),
                    It.IsAny<string[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string key, Func<CancellationToken, ValueTask<IEnumerable<InstructorEnrollmentStatisticsDto>?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                    return factory(ct);
                });
        }

        [Fact]
        public async Task Handle_ShouldReturnEnrollmentStatistics_WhenDataExists()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>
            {
                new() { Period = "Jan", EnrollmentCount = 10 },
                new() { Period = "Feb", EnrollmentCount = 15 },
                new() { Period = "Mar", EnrollmentCount = 20 }
            };

            _repoMock
                .Setup(r => r.GetInstructorEnrollmentStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            var query = new GetInstructorEnrollmentStatisticsQuery(instructorId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(expectedData);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoDataExists()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>();

            _repoMock
                .Setup(r => r.GetInstructorEnrollmentStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            var query = new GetInstructorEnrollmentStatisticsQuery(instructorId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturn12Months_WhenRepositoryReturns12Months()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>();
            for (int i = 1; i <= 12; i++)
            {
                expectedData.Add(new InstructorEnrollmentStatisticsDto 
                { 
                    Period = "Month" + i, 
                    EnrollmentCount = i * 5 
                });
            }

            _repoMock
                .Setup(r => r.GetInstructorEnrollmentStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            var query = new GetInstructorEnrollmentStatisticsQuery(instructorId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(12);
        }

        [Fact]
        public async Task Handle_ShouldPassInstructorIdToRepository()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var expectedData = new List<InstructorEnrollmentStatisticsDto>
            {
                new() { Period = "Jan", EnrollmentCount = 5 }
            };

            _repoMock
                .Setup(r => r.GetInstructorEnrollmentStatisticsAsync(instructorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedData);

            var query = new GetInstructorEnrollmentStatisticsQuery(instructorId);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.GetInstructorEnrollmentStatisticsAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
