using API.Endpoints;
using Application.DTOs.InstructorDashboard;
using Application.Features.InstructorDashboard.Queries.GetInstructorStatistics;
using Application.Features.Instructors.Queries.GetCurrentInstructor;
using Application.DTOs.Instructor;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace API.Tests.Endpoints
{
    public class InstructorDashboardEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public InstructorDashboardEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetInstructorStatistics_ShouldReturnOk_WithStatistics()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var currentInstructor = new InstructorPrivateResponseDto
            {
                Id = instructorId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 1240,
                MonthlyEarnings = 12450.00m,
                InstructorRating = 4.92m,
                CoursesCreated = 4
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentInstructorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentInstructor);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInstructorStatisticsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await InstructorDashboardEndpoints.GetInstructorStatistics(_mediatorMock.Object);

            // Assert
            var okResult = result as Ok<InstructorStatisticsDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedStats);
        }

        [Fact]
        public async Task GetInstructorStatistics_ShouldReturnNotFound_WhenInstructorNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentInstructorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InstructorPrivateResponseDto?)null);

            // Act
            var result = await InstructorDashboardEndpoints.GetInstructorStatistics(_mediatorMock.Object);

            // Assert
            result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task GetInstructorStatistics_ShouldCallGetCurrentInstructor_Once()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var currentInstructor = new InstructorPrivateResponseDto
            {
                Id = instructorId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 100,
                MonthlyEarnings = 5000.00m,
                InstructorRating = 4.5m,
                CoursesCreated = 2
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentInstructorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentInstructor);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInstructorStatisticsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            // Act
            await InstructorDashboardEndpoints.GetInstructorStatistics(_mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetCurrentInstructorQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInstructorStatistics_ShouldCallGetInstructorStatistics_Once()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var currentInstructor = new InstructorPrivateResponseDto
            {
                Id = instructorId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 100,
                MonthlyEarnings = 5000.00m,
                InstructorRating = 4.5m,
                CoursesCreated = 2
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentInstructorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentInstructor);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInstructorStatisticsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            // Act
            await InstructorDashboardEndpoints.GetInstructorStatistics(_mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetInstructorStatisticsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInstructorStatistics_ShouldPassInstructorId_ToQuery()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var currentInstructor = new InstructorPrivateResponseDto
            {
                Id = instructorId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            var expectedStats = new InstructorStatisticsDto
            {
                EnrolledStudents = 100,
                MonthlyEarnings = 5000.00m,
                InstructorRating = 4.5m,
                CoursesCreated = 2
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCurrentInstructorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(currentInstructor);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetInstructorStatisticsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedStats);

            // Act
            await InstructorDashboardEndpoints.GetInstructorStatistics(_mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetInstructorStatisticsQuery>(q => q.InstructorId == instructorId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}