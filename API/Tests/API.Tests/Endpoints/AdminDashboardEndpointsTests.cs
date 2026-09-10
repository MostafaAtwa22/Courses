using API.Endpoints;
using Application.DTOs.AdminDashboard;
using Application.Features.AdminDashboard.Queries.GetRoleStatistics;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace API.Tests.Endpoints
{
    public class AdminDashboardEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public AdminDashboardEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetRoleStatistics_ShouldReturnOk_WithRoleStatistics()
        {
            // Arrange
            var expectedStats = new RoleStatisticsDto
            {
                SuperAdminCount = 2,
                SuperAdminChange = 0,
                AdminCount = 5,
                AdminChange = 0,
                InstructorCount = 15,
                InstructorChange = 0,
                StudentCount = 150,
                StudentChange = 0
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRoleStatisticsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await AdminDashboardEndpoints.GetRoleStatistics(_mediatorMock.Object);

            // Assert
            var okResult = result as Ok<RoleStatisticsDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedStats);
        }

        [Fact]
        public async Task GetRoleStatistics_ShouldCallMediator_Once()
        {
            // Arrange
            var expectedStats = new RoleStatisticsDto
            {
                SuperAdminCount = 1,
                SuperAdminChange = 0,
                AdminCount = 3,
                AdminChange = 0,
                InstructorCount = 10,
                InstructorChange = 0,
                StudentCount = 100,
                StudentChange = 0
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetRoleStatisticsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            // Act
            await AdminDashboardEndpoints.GetRoleStatistics(_mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetRoleStatisticsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
