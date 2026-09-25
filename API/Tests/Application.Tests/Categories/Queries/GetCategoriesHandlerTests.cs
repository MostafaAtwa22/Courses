using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Category;
using Application.Features.Categories.Queries.GetAll;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Categories.Queries
{
    public class GetCategoriesHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly GetCategoriesQueryHandler _handler;

        public GetCategoriesHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _cacheMock = new Mock<IAppCache>();
            _handler = new GetCategoriesQueryHandler(_categoryRepositoryMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedResults_WhenCalledWithValidParams()
        {
            // Arrange
            var queryParams = new QueryParams 
            {
                PageNumber = 1,
                PageSize = 10
            };

            var expected = new PaginatedResult<CategoryResponseDto>
            {
                Items = new List<CategoryResponseDto>
                {
                    new CategoryResponseDto { Id = Guid.NewGuid(), Name = "Category 1" },
                    new CategoryResponseDto { Id = Guid.NewGuid(), Name = "Category 2" }
                },
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10
            };

            _categoryRepositoryMock
                .Setup(repo => repo.GetAllAsync(queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            _cacheMock
                .Setup(cache => cache.GetOrCreateAsync<PaginatedResult<CategoryResponseDto>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<PaginatedResult<CategoryResponseDto>?>>>(),
                    It.IsAny<CacheOptions>(),
                    It.IsAny<string[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string key, Func<CancellationToken, ValueTask<PaginatedResult<CategoryResponseDto>?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                    var result = factory(ct);
                    return factory(ct);
                });

            var query = new GetCategoriesQuery(queryParams);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            
            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(expected.TotalCount);
            result.Items.Should().HaveCount(expected.Items.Count);
            result.Items[0].Name.Should().Be("Category 1");
        }
    }
}