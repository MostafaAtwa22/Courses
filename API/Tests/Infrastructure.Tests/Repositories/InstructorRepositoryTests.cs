using System.Data;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Application.DTOs.Instructor;
using Domain.Entities.Identity;
using Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;

namespace Infrastructure.Tests.Repositories;

public class InstructorRepositoryTests
{
    private readonly Mock<IDbConnectionFactory> _factoryMock;
    private readonly Mock<IDbConnection> _connectionMock;
    private readonly Mock<IOptions<UrlsOptions>> _urlsOptionsMock;
    private readonly InstructorRepository _repository;

    public InstructorRepositoryTests()
    {
        _factoryMock = new Mock<IDbConnectionFactory>();
        _connectionMock = new Mock<IDbConnection>();
        _urlsOptionsMock = new Mock<IOptions<UrlsOptions>>();

        _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(_connectionMock.Object);

        _urlsOptionsMock.Setup(x => x.Value).Returns(new UrlsOptions { API = "https://api.example.com" });

        _repository = new InstructorRepository(_factoryMock.Object, _urlsOptionsMock.Object);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnInstructor_WhenExists()
    {
        // Arrange
        var userId = "user123";
        var expected = new Instructor { Id = Guid.NewGuid(), UserId = userId };

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Instructor>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expected);

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var userId = "user123";

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Instructor>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync((Instructor?)null);

        // Act
        var result = await _repository.GetByUserIdAsync(userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetEntityByIdAsync_ShouldReturnInstructor_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new Instructor { Id = id };

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Instructor>(
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
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<Instructor>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync((Instructor?)null);

        // Act
        var result = await _repository.GetEntityByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPublicByIdAsync_ShouldReturnDto_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new InstructorPublicResponseDto { Id = id };

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorPublicResponseDto>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expected);

        // Act
        var result = await _repository.GetPublicByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetPublicByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorPublicResponseDto>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync((InstructorPublicResponseDto?)null);

        // Act
        var result = await _repository.GetPublicByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPrivateByIdAsync_ShouldReturnDto_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new InstructorPrivateResponseDto { Id = id };

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorPrivateResponseDto>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync(expected);

        // Act
        var result = await _repository.GetPrivateByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetPrivateByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        _connectionMock
            .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<InstructorPrivateResponseDto>(
                It.IsAny<string>(), It.IsAny<object>(), null, null, null))
            .ReturnsAsync((InstructorPrivateResponseDto?)null);

        // Act
        var result = await _repository.GetPrivateByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnInstructorId()
    {
        // Arrange
        var instructor = new Instructor { Bio = "Test Bio" };
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
        var result = await _repository.CreateAsync(instructor);

        // Assert
        result.Should().Be(instructor.Id);
        executeCallCount.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ShouldExecuteWithCorrectArguments()
    {
        // Arrange
        var instructor = new Instructor { Id = Guid.NewGuid(), Bio = "Updated Bio" };
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
        await _repository.UpdateAsync(instructor);

        // Assert
        executeCallCount.Should().Be(1);
        instructor.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
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
