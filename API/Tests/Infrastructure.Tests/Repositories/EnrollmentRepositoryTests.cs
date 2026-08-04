using System.Data;
using Application.Common.Interfaces;
using Infrastructure.Repositories;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Dapper;

namespace Infrastructure.Tests.Repositories
{
    public class EnrollmentRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly EnrollmentRepository _repository;

        public EnrollmentRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new EnrollmentRepository(_factoryMock.Object);
        }

        [Fact]
        public async Task IsEnrolledAsync_ShouldReturnTrue_WhenStudentIsEnrolled()
        {
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            var result = await _repository.IsEnrolledAsync(studentId, courseId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsEnrolledAsync_ShouldReturnFalse_WhenStudentIsNotEnrolled()
        {
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(0);

            var result = await _repository.IsEnrolledAsync(studentId, courseId);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsEnrolledByUserIdAsync_ShouldReturnTrue_WhenUserIsEnrolled()
        {
            var userId = "user123";
            var courseId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            var result = await _repository.IsEnrolledByUserIdAsync(userId, courseId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsEnrolledByUserIdAsync_ShouldReturnFalse_WhenUserIsNotEnrolled()
        {
            var userId = "user123";
            var courseId = Guid.NewGuid();

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<int>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(0);

            var result = await _repository.IsEnrolledByUserIdAsync(userId, courseId);

            result.Should().BeFalse();
        }
    }
}
