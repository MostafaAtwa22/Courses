using API.Endpoints;
using Application.DTOs.Progress;
using Application.Features.Progress.Commands.MarkComplete;
using Application.Features.Progress.Commands.MarkIncomplete;
using Application.Features.Progress.Queries.GetCourseProgress;
using Application.Features.Progress.Queries.GetMyCoursesProgress;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class ProgressEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public ProgressEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetCourseProgress_ShouldReturnOk_WhenProgressExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedProgress = new CourseProgressDto
            {
                CourseId = courseId,
                CompletedCount = 2,
                TotalCount = 5,
                PercentComplete = 40,
                CompletedContentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseProgressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProgress);

            // Act
            var result = await ProgressEndpoints.GetCourseProgress(courseId, _mediatorMock.Object);

            // Assert
            var okResult = result as Ok<CourseProgressDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedProgress);
        }

        [Fact]
        public async Task GetMyCoursesProgress_ShouldReturnOk_WithProgressList()
        {
            // Arrange
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var expectedProgress = new List<CourseProgressSummaryDto>
            {
                new CourseProgressSummaryDto { CourseId = courseId1, CompletedCount = 2, TotalCount = 5, PercentComplete = 40 },
                new CourseProgressSummaryDto { CourseId = courseId2, CompletedCount = 3, TotalCount = 3, PercentComplete = 100 }
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMyCoursesProgressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProgress);

            // Act
            var result = await ProgressEndpoints.GetMyCoursesProgress(_mediatorMock.Object);

            // Assert
            var okResult = result as Ok<IReadOnlyList<CourseProgressSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedProgress);
        }

        [Fact]
        public async Task GetMyCoursesProgress_ShouldReturnOk_WithEmptyList_WhenNoEnrollments()
        {
            // Arrange
            var expectedProgress = new List<CourseProgressSummaryDto>();
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetMyCoursesProgressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProgress);

            // Act
            var result = await ProgressEndpoints.GetMyCoursesProgress(_mediatorMock.Object);

            // Assert
            var okResult = result as Ok<IReadOnlyList<CourseProgressSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task MarkContentComplete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new MarkProgressRequestDto { ContentId = Guid.NewGuid(), CourseId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkContentCompleteCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProgressEndpoints.MarkContentComplete(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task MarkContentIncomplete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new MarkProgressRequestDto { ContentId = Guid.NewGuid(), CourseId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkContentIncompleteCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProgressEndpoints.MarkContentIncomplete(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCourseProgress_ShouldSendQuery_WithCorrectCourseId()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedProgress = new CourseProgressDto { CourseId = courseId };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseProgressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedProgress);

            // Act
            await ProgressEndpoints.GetCourseProgress(courseId, _mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<GetCourseProgressQuery>(q => q.CourseId == courseId), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task MarkContentComplete_ShouldSendCommand_WithCorrectDto()
        {
            // Arrange
            var dto = new MarkProgressRequestDto { ContentId = Guid.NewGuid(), CourseId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkContentCompleteCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await ProgressEndpoints.MarkContentComplete(dto, _mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<MarkContentCompleteCommand>(c => c.Dto == dto), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task MarkContentIncomplete_ShouldSendCommand_WithCorrectDto()
        {
            // Arrange
            var dto = new MarkProgressRequestDto { ContentId = Guid.NewGuid(), CourseId = Guid.NewGuid() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<MarkContentIncompleteCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await ProgressEndpoints.MarkContentIncomplete(dto, _mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(
                It.Is<MarkContentIncompleteCommand>(c => c.Dto == dto), 
                It.IsAny<CancellationToken>()), 
                Times.Once);
        }
    }
}
