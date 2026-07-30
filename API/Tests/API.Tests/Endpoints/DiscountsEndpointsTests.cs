using API.Endpoints;
using Application.DTOs.Course;
using Application.Features.Discount.Commands.Create;
using Application.Features.Discount.Commands.Delete;
using Application.Features.Discount.Commands.Update;
using Application.Features.Discount.Queries.GetDiscounts;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace API.Tests.Endpoints
{
    public class DiscountsEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public DiscountsEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task AddDiscount_ShouldReturnCreatedAtRoute_WhenSuccessful()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var request = new CreateCourseDiscountDto { Percentage = 20, StartTime = DateTimeOffset.UtcNow, EndTime = DateTimeOffset.UtcNow.AddDays(30) };
            var discountId = Guid.NewGuid();
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDiscountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(discountId);

            // Act
            var result = await DiscountsEndpoints.AddDiscount(courseId, request, _mediatorMock.Object);

            // Assert
            var createdResult = result.Result as CreatedAtRoute<Guid>;
            createdResult.Should().NotBeNull();
            createdResult!.RouteName.Should().Be(nameof(DiscountsEndpoints.GetCourseDiscounts));
            createdResult.RouteValues.Should().ContainKey("courseId").WhoseValue.Should().Be(courseId);
            createdResult.Value.Should().Be(discountId);
        }

        [Fact]
        public async Task GetCourseDiscounts_ShouldReturnOk_WithDiscounts()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedDiscounts = new List<CourseDiscountDto>
            {
                new CourseDiscountDto { Id = Guid.NewGuid(), Percentage = 20, StartTime = DateTimeOffset.UtcNow, EndTime = DateTimeOffset.UtcNow.AddDays(30) },
                new CourseDiscountDto { Id = Guid.NewGuid(), Percentage = 30, StartTime = DateTimeOffset.UtcNow, EndTime = DateTimeOffset.UtcNow.AddDays(30) }
            };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseDiscountsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDiscounts);

            // Act
            var result = await DiscountsEndpoints.GetCourseDiscounts(courseId, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<IEnumerable<CourseDiscountDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedDiscounts);
        }

        [Fact]
        public async Task UpdateDiscount_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCourseDiscountDto { Percentage = 25, StartTime = DateTimeOffset.UtcNow, EndTime = DateTimeOffset.UtcNow.AddDays(30) };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDiscountCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await DiscountsEndpoints.UpdateDiscount(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteDiscount_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDiscountCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await DiscountsEndpoints.DeleteDiscount(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
