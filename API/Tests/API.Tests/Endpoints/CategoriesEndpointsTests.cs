using API.Endpoints;
using Application.Common.Models;
using Application.DTOs.Category;
using Application.Features.Categories.Commands.Create;
using Application.Features.Categories.Commands.Delete;
using Application.Features.Categories.Commands.Update;
using Application.Features.Categories.Queries.GetAll;
using Application.Features.Categories.Queries.GetById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class CategoriesEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public CategoriesEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetCategories_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var queryParams = new QueryParams();
            var expectedResult = new PaginatedResult<CategoryResponseDto>(new List<CategoryResponseDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoriesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CategoriesEndpoints.GetCategories(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CategoryResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCategoryById_ShouldReturnOk_WhenCategoryExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedCategory = new CategoryResponseDto { Id = id, Name = "Test Category", Slug = "test-category" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await CategoriesEndpoints.GetCategoryById(id, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<CategoryResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedCategory);
        }

        [Fact]
        public async Task GetCategoryById_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CategoryResponseDto)null!);

            // Act
            var result = await CategoriesEndpoints.GetCategoryById(id, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnCreatedAtRoute_WhenSuccessful()
        {
            // Arrange
            var request = new CategoryCreateDto { Name = "New Category" };
            var newId = Guid.NewGuid();
            var expectedCategory = new CategoryResponseDto { Id = newId, Name = "New Category" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCategoryCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCategory);

            // Act
            var result = await CategoriesEndpoints.CreateCategory(request, _mediatorMock.Object);

            // Assert
            var createdResult = result.Result as CreatedAtRoute<CategoryResponseDto>;
            createdResult.Should().NotBeNull();
            createdResult!.RouteName.Should().Be(nameof(CategoriesEndpoints.GetCategoryById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(newId);
            createdResult.Value.Should().Be(expectedCategory);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new CategoryUpdateDto { Name = "Updated Category" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await CategoriesEndpoints.UpdateCategory(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await CategoriesEndpoints.DeleteCategory(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
