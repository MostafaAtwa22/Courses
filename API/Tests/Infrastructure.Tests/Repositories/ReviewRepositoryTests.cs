using System.Data;
using Application.Common.Interfaces;
using Application.Common.Options;
using Application.DTOs.Review;
using Infrastructure.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;
using Domain.Entities;
using Application.Common.Models;

namespace Infrastructure.Tests.Repositories
{
    public class ReviewRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly Mock<IOptions<UrlsOptions>> _urlsOptionsMock;
        private readonly ReviewRepository _repository;

        public ReviewRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();
            _urlsOptionsMock = new Mock<IOptions<UrlsOptions>>();

            _urlsOptionsMock.Setup(o => o.Value).Returns(new UrlsOptions { API = "https://localhost:7297" });

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new ReviewRepository(_factoryMock.Object, _urlsOptionsMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnReview_WhenExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = new ReviewResponseDto { Id = id, Headline = "Great" };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<ReviewResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expected);

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task IsStudentEnrolledAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.IsStudentEnrolledAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasStudentReviewedAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.HasStudentReviewedAsync(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            result.Should().BeTrue();
        }

        // [Fact]
        // public async Task GetStudentIdByUserIdAsync_ShouldReturnId_WhenExists()
        // {
        //     // Arrange
        //     var userId = "user-123";
        //     var expectedId = Guid.NewGuid();
        // 
        //     _connectionMock
        //         .SetupDapperAsync(c => c.ExecuteScalarAsync<Guid?>(
        //             It.IsAny<string>(), It.IsAny<object>(), null, null, null))
        //         .ReturnsAsync(expectedId);
        // 
        //     // Act
        //     var result = await _repository.GetStudentIdByUserIdAsync(userId);
        // 
        //     // Assert
        //     result.Should().Be(expectedId);
        // }

        [Fact]
        public async Task UpdateAsync_ShouldExecuteSuccessfully()
        {
            // Arrange
            var review = new Review { Id = Guid.NewGuid(), CourseId = Guid.NewGuid(), Rating = 5 };
            var executeCallCount = 0;

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(() => {
                    executeCallCount++;
                    return 1;
                });

            // Act
            await _repository.UpdateAsync(review);

            // Assert
            executeCallCount.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnNewGuid()
        {
            // Arrange
            var review = new Review { Headline = "Test", CourseId = Guid.NewGuid() };
            var executeCallCount = 0;
            
            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(() => {
                    executeCallCount++;
                    return 1;
                });

            // Act
            var id = await _repository.CreateAsync(review);

            // Assert
            id.Should().NotBeEmpty();
            executeCallCount.Should().BeGreaterThanOrEqualTo(1);
        }
    }
}
