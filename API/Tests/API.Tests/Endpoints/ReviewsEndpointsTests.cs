using API.Endpoints;
using Application.Common.Models;
using Application.DTOs.Review;
using Application.Features.Reviews.Commands.Create;
using Application.Features.Reviews.Commands.Delete;
using Application.Features.Reviews.Commands.Update;
using Application.Features.Reviews.Queries.GetByCourse;
using Application.Features.Reviews.Queries.GetById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class ReviewsEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public ReviewsEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetReviewsByCourse_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams();
            var expectedResult = new PaginatedResult<ReviewResponseDto>(new List<ReviewResponseDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReviewsByCourseQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await ReviewsEndpoints.GetReviewsByCourse(courseId, queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<ReviewResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetReviewById_ShouldReturnOk_WhenReviewExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedReview = new ReviewResponseDto { Id = id, Rating = 5, Comment = "Great course!" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReviewByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReview);

            // Act
            var result = await ReviewsEndpoints.GetReviewById(id, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<ReviewResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedReview);
        }

        [Fact]
        public async Task GetReviewById_ShouldReturnNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetReviewByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ReviewResponseDto)null!);

            // Act
            var result = await ReviewsEndpoints.GetReviewById(id, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateReview_ShouldReturnCreatedAtRoute_WhenSuccessful()
        {
            // Arrange
            var dto = new ReviewCreateDto { CourseId = Guid.NewGuid(), Rating = 5, Comment = "Great course!" };
            var newId = Guid.NewGuid();
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateReviewCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);

            // Act
            var result = await ReviewsEndpoints.CreateReview(dto, _mediatorMock.Object);

            // Assert
            var createdResult = result as CreatedAtRoute<Guid>;
            createdResult.Should().NotBeNull();
            createdResult!.RouteName.Should().Be(nameof(ReviewsEndpoints.GetReviewById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(newId);
            createdResult.Value.Should().Be(newId);
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new ReviewUpdateDto { Rating = 4, Comment = "Updated review" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateReviewCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ReviewsEndpoints.UpdateReview(id, dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteReview_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteReviewCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ReviewsEndpoints.DeleteReview(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
