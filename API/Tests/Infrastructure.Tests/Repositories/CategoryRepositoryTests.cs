using System.Data;
using Application.Common.Interfaces;
using Application.DTOs.Category;
using Infrastructure.Repositories;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;

namespace Infrastructure.Tests.Repositories
{
    public class CategoryRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new CategoryRepository(_factoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCategory_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new CategoryResponseDto { Id = id, Name = "Test", Slug = "test" };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CategoryResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            result.Name.Should().Be("Test");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CategoryResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync((CategoryResponseDto?)null);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnNewGuid()
        {
            // Arrange
            var dto = new CategoryCreateDto { Name = "New Category", Slug = "new-category" };
            var executeCallCount = 0;

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(() =>
                {
                    executeCallCount++;
                    return 1;
                });

            // Act
            var result = await _repository.CreateAsync(dto);

            // Assert
            result.Should().NotBeEmpty();
            executeCallCount.Should().Be(1);
        }

        [Fact]
        public async Task UpdateAsync_ShouldExecuteWithCorrectArguments()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new CategoryUpdateDto { Name = "Updated Name", Slug = "updated-name" };
            var executeCallCount = 0;

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(() =>
                {
                    executeCallCount++;
                    return 1;
                });

            // Act
            await _repository.UpdateAsync(id, dto);

            // Assert
            executeCallCount.Should().Be(1);
        }

        [Fact]
        public async Task DeleteAsync_ShouldExecuteOnce()
        {
            // Arrange
            var id = Guid.NewGuid();
            var executeCallCount = 0;

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(() =>
                {
                    executeCallCount++;
                    return 1;
                });

            // Act
            await _repository.DeleteAsync(id);

            // Assert
            executeCallCount.Should().Be(1);
        }
    }
}