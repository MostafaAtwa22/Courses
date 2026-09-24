using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Models;
using Application.DTOs.Section;
using Application.Features.Sections.Queries.GetByCourseId;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Sections.Queries
{
    public class GetSectionsByCourseIdQueryHandlerTests
    {
        private readonly Mock<ISectionRepository> _sectionRepositoryMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly GetSectionsByCourseIdQueryHandler _handler;

        public GetSectionsByCourseIdQueryHandlerTests()
        {
            _sectionRepositoryMock = new Mock<ISectionRepository>();
            _cacheMock = new Mock<IAppCache>();
            _handler = new GetSectionsByCourseIdQueryHandler(_sectionRepositoryMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedSections()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams();
            var query = new GetSectionsByCourseIdQuery(courseId, queryParams);
            var expectedSections = new PaginatedResult<SectionResponseDto>
            {
                Items = new List<SectionResponseDto> { new SectionResponseDto { Id = Guid.NewGuid(), Title = "Section 1", Order = 1 } },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _sectionRepositoryMock
                .Setup(repo => repo.GetByCourseIdAsync(courseId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSections);

            _cacheMock
                .Setup(cache => cache.GetOrCreateAsync<PaginatedResult<SectionResponseDto>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<PaginatedResult<SectionResponseDto>?>>>(),
                    It.IsAny<CacheOptions>(),
                    It.IsAny<string[]>(),
                    It.IsAny<CancellationToken>()))
                .Returns((string key, Func<CancellationToken, ValueTask<PaginatedResult<SectionResponseDto>?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                    var result = factory(ct);
                    return factory(ct);
                });

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedSections);
        }
    }
}
