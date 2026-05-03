using System.Data;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Content;
using Domain.Entities;
using Infrastructure.Repositories;
using Application.Common.Options;
using Moq;
using Moq.Dapper;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Dapper;

namespace Infrastructure.Tests.Repositories
{
    public class ContentRepositoryTests
    {
        private readonly Mock<IDbConnectionFactory> _factoryMock;
        private readonly Mock<IDbConnection> _connectionMock;
        private readonly Mock<IOptions<UrlsOptions>> _urlsOptionsMock;
        private readonly ContentRepository _repository;

        public ContentRepositoryTests()
        {
            _factoryMock = new Mock<IDbConnectionFactory>();
            _connectionMock = new Mock<IDbConnection>();
            _urlsOptionsMock = new Mock<IOptions<UrlsOptions>>();

            _urlsOptionsMock.Setup(x => x.Value).Returns(new UrlsOptions { API = "http://localhost:5000" });

            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(_connectionMock.Object);

            _repository = new ContentRepository(_factoryMock.Object, _urlsOptionsMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnContent_WhenExists()
        {
            var id = Guid.NewGuid();
            var expected = new ContentResponseDto { Id = id, Title = "Video 1" };

            _connectionMock
                .SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<ContentResponseDto>(
                    It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(expected);

            var result = await _repository.GetByIdAsync(id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(id);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnId()
        {
            var content = new Content { Id = Guid.NewGuid(), Title = "New Video" };

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(1);

            var result = await _repository.CreateAsync(content);

            result.Should().Be(content.Id);
        }

        [Fact]
        public async Task UpdateAsync_ShouldExecuteOnce()
        {
            var content = new Content { Id = Guid.NewGuid(), Title = "Updated Video" };
            var callCount = 0;

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(() => { callCount++; return 1; });

            await _repository.UpdateAsync(content);

            callCount.Should().Be(1);
        }

        [Fact]
        public async Task DeleteAsync_ShouldExecuteOnce()
        {
            var id = Guid.NewGuid();
            var callCount = 0;

            _connectionMock
                .SetupDapperAsync(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(() => { callCount++; return 1; });

            await _repository.DeleteAsync(id);

            callCount.Should().Be(1);
        }

        [Fact]
        public async Task GetBySectionAsync_ShouldVerifyConnection()
        {
            var sectionId = Guid.NewGuid();
            var expectedException = new Exception("DB Hit");

            _connectionMock.Setup(c => c.State).Returns(ConnectionState.Open);
            _connectionMock.Setup(c => c.CreateCommand()).Throws(expectedException);

            var act = async () => await _repository.GetBySectionAsync(sectionId);

            var ex = await act.Should().ThrowAsync<Exception>();
            ex.WithMessage("DB Hit");
        }

        [Fact]
        public async Task GetByCourseAsync_ShouldVerifyConnection()
        {
            var courseId = Guid.NewGuid();
            var expectedException = new Exception("DB Hit");

            _connectionMock.Setup(c => c.State).Returns(ConnectionState.Open);
            _connectionMock.Setup(c => c.CreateCommand()).Throws(expectedException);

            var act = async () => await _repository.GetByCourseAsync(courseId, new QueryParams());

            var ex = await act.Should().ThrowAsync<Exception>();
            ex.WithMessage("DB Hit");
        }
    }
}
