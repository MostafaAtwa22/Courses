using Application.Common.Interfaces;
using Application.DTOs.Category;
using Application.Features.Categories.Queries.GetById;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Categories.Queries
{
    public class GetCategoryByIdHandlerTests
    {
        private readonly Mock<ICategoryRepository> _repoMock;
        private readonly GetCategoryByIdQueryHandler _handler;

        public GetCategoryByIdHandlerTests()
        {
            _repoMock = new Mock<ICategoryRepository>();
            _handler = new GetCategoryByIdQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_Returns_Category_When_Category_Exists()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new CategoryResponseDto { Id = categoryId, Name = "Test Category" };
            _repoMock.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            // Act
            var result = await _handler.Handle(new GetCategoryByIdQuery(categoryId), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(categoryId);
            result.Name.Should().Be("Test Category");
        }

        [Fact]
        public async Task Handle_Returns_Null_When_Category_Does_Not_Exist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _repoMock.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync((CategoryResponseDto?)null);

            // Act
            var result = await _handler.Handle(new GetCategoryByIdQuery(categoryId), CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}