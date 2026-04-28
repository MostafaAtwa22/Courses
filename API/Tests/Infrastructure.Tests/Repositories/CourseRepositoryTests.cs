using System.Data;
using Application.Common.Interfaces;
using Application.Common.Options;
using Application.DTOs.Course;
using Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;
using Domain.Entities;

namespace Infrastructure.Tests.Repositories
{
    public class CourseRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly Mock<IOptions<UrlsOptions>> _urlsOptionsMock;
        private readonly CourseRepository _repository;

        public CourseRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();
            _urlsOptionsMock = new Mock<IOptions<UrlsOptions>>();

            _urlsOptionsMock.Setup(o => o.Value).Returns(new UrlsOptions { API = "https://localhost:7297" });

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new CourseRepository(_factoryMock.Object, _urlsOptionsMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCourse_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new CourseResponseDto { Id = id, Title = "Test Course", Description = "Test" };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
            result.Title.Should().Be("Test Course");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var id = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync((CourseResponseDto?)null);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().BeNull();
        }
        [Fact]
        public async Task CreateAsync_ShouldReturnNewGuid()
        {
            // Arrange
            var course = new Course { Title = "Test" };
            
            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act
            var id = await _repository.CreateAsync(course);

            // Assert
            id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task UpdateAsync_ShouldExecuteUpdate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var course = new Course { Id = id, Title = "Updated" };

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act
            await _repository.UpdateAsync(course);

            // Assert
            // Assert successful execution
        }

        [Fact]
        public async Task DeleteAsync_ShouldExecuteDelete()
        {
            // Arrange
            var id = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act
            await _repository.DeleteAsync(id);

            // Assert
            // Assert successful execution
        }
    }
}
