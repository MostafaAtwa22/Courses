using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Section;
using Application.Features.Sections.Queries.GetAll;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Sections.Queries
{
    public class GetSectionsQueryHandlerTests
    {
        private readonly Mock<ISectionRepository> _sectionRepositoryMock;
        private readonly GetSectionsQueryHandler _handler;

        public GetSectionsQueryHandlerTests()
        {
            _sectionRepositoryMock = new Mock<ISectionRepository>();
            _handler = new GetSectionsQueryHandler(_sectionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedSections()
        {
            // Arrange
            var queryParams = new QueryParams();
            var query = new GetSectionsQuery(queryParams);
            var expectedSections = new PaginatedResult<SectionResponseDto>
            {
                Items = new List<SectionResponseDto> { new SectionResponseDto { Id = Guid.NewGuid(), Title = "Section 1", Order = 1 } },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10
            };

            _sectionRepositoryMock
                .Setup(repo => repo.GetAllAsync(queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSections);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedSections);
        }
    }
}
