using System.Data;
using Application.Common.Interfaces;
using Application.DTOs.Section;
using Infrastructure.Repositories;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;
using Domain.Entities;
using Application.Common.Models;

namespace Infrastructure.Tests.Repositories
{
    public class SectionRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly SectionRepository _repository;

        public SectionRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new SectionRepository(_factoryMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnSection_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new SectionResponseDto { Id = id, Title = "Test Section", Order = 1 };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<SectionResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            result.Title.Should().Be("Test Section");
            result.Order.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<SectionResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync((SectionResponseDto?)null);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnId()
        {
            // Arrange
            var section = new Section 
            { 
                Id = Guid.NewGuid(),
                Title = "New Section", 
                Order = 1,
                CourseId = Guid.NewGuid()
            };

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.CreateAsync(section);

            // Assert
            result.Should().Be(section.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldExecuteOnce()
        {
            // Arrange
            var section = new Section 
            { 
                Id = Guid.NewGuid(),
                Title = "Updated Section", 
                Order = 2 
            };
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
            await _repository.UpdateAsync(section);

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

        [Fact]
        public async Task GetByCourseIdAsync_ShouldExecuteQueryMultiple()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams();
            
            var expectedException = new Exception("Database call reached");

            _connectionMock
                .Setup(c => c.State)
                .Returns(ConnectionState.Open);

            _connectionMock
                .Setup(c => c.CreateCommand())
                .Throws(expectedException);

            // Act
            var act = async () => await _repository.GetByCourseIdAsync(courseId, queryParams);

            // Assert
            var exception = await act.Should().ThrowAsync<Exception>();
            exception.WithMessage("Database call reached");
            
            _factoryMock.Verify(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
