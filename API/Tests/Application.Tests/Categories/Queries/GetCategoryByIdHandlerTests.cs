using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
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
        private readonly Mock<IAppCache> _cacheMock;
        private readonly GetCategoryByIdQueryHandler _handler;

        public GetCategoryByIdHandlerTests()
        {
            _repoMock = new Mock<ICategoryRepository>();
            _cacheMock = new Mock<IAppCache>();
            _handler = new GetCategoryByIdQueryHandler(_repoMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsCategory_WhenCategoryExists()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new CategoryResponseDto { Id = categoryId, Name = "Test Category" };
            _repoMock.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync(category);

            _cacheMock
                .Setup(cache => cache.GetOrCreateAsync<CategoryResponseDto>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<CategoryResponseDto?>>>(),
                    It.IsAny<CacheOptions>(),
                    It.IsAny<string[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string key, Func<CancellationToken, ValueTask<CategoryResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                    var result = factory(ct);
                    return factory(ct);
                });

            // Act
            var result = await _handler.Handle(new GetCategoryByIdQuery(categoryId), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(categoryId);
            result.Name.Should().Be("Test Category");
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            _repoMock.Setup(repo => repo.GetByIdAsync(categoryId, It.IsAny<CancellationToken>())).ReturnsAsync((CategoryResponseDto?)null);

            _cacheMock
                .Setup(cache => cache.GetOrCreateAsync<CategoryResponseDto>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<CategoryResponseDto?>>>(),
                    It.IsAny<CacheOptions>(),
                    It.IsAny<string[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string key, Func<CancellationToken, ValueTask<CategoryResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                    var result = factory(ct);
                    return factory(ct);
                });

            // Act
            var result = await _handler.Handle(new GetCategoryByIdQuery(categoryId), CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}