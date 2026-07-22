using System.Data;
using Application.Common.Interfaces;
using Application.DTOs.Course;
using Infrastructure.Repositories;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;
using Domain.Entities;

namespace Infrastructure.Tests.Repositories;

public class CourseDiscountRepositoryTests
{
    private readonly Mock<IDbConnectionFactory> _factoryMock;
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly CourseDiscountRepository _repository;

    public CourseDiscountRepositoryTests()
    {
        _factoryMock = new Mock<IDbConnectionFactory>();
        _connectionMock = new Mock<IDbConnection>();

        _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_connectionMock.Object);

        _repository = new CourseDiscountRepository(_factoryMock.Object);
    }

    [Fact]
    public async Task GetByCourseIdAsync_ShouldReturnDiscounts_WhenExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var expected = new List<CourseDiscountDto> 
        { 
            new() { Id = Guid.NewGuid(), CourseId = courseId, Percentage = 20m },
            new() { Id = Guid.NewGuid(), CourseId = courseId, Percentage = 30m }
        };

        _connectionMock
            .SetupDapperAsync(c => c.QueryAsync<CourseDiscountDto>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expected);

        // Act
        var result = await _repository.GetByCourseIdAsync(courseId);

        // Assert
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnGuid()
    {
        // Arrange
        var discount = new CourseDiscount 
        { 
            Id = Guid.NewGuid(),
            Percentage = 20m,
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddDays(7)
        };
        var executeCallCount = 0;

        _connectionMock
            .SetupDapperAsync(c => c.QuerySingleAsync<Guid>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync((string sql, object param) =>
            {
                executeCallCount++;
                return ((CourseDiscount)param).Id;
            });

        // Act
        var result = await _repository.AddAsync(discount);

        // Assert
        result.Should().Be(discount.Id);
        executeCallCount.Should().Be(1);
    }

    [Fact]
    public async Task GetEntityByIdAsync_ShouldReturnDiscount_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new CourseDiscount { Id = id, Percentage = 20m };

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseDiscount>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expected);

        // Act
        var result = await _repository.GetEntityByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetEntityByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<CourseDiscount>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync((CourseDiscount?)null);

        // Act
        var result = await _repository.GetEntityByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldExecuteWithCorrectArguments()
    {
        // Arrange
        var id = Guid.NewGuid();
        var discount = new CourseDiscount { Id = id, Percentage = 30m };
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
        await _repository.UpdateAsync(discount);

        // Assert
        executeCallCount.Should().Be(1);
        discount.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
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
    public async Task DeactivateExpiredAsync_ShouldExecuteOnce()
    {
        // Arrange
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
        await _repository.DeactivateExpiredAsync();

        // Assert
        executeCallCount.Should().Be(1);
    }
}
