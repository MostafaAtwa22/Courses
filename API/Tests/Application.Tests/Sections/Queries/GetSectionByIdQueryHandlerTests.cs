using Application.Common.Interfaces;
using Application.DTOs.Section;
using Application.Features.Sections.Queries.GetById;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Sections.Queries
{
    public class GetSectionByIdQueryHandlerTests
    {
        private readonly Mock<ISectionRepository> _sectionRepositoryMock;
        private readonly GetSectionByIdQueryHandler _handler;

        public GetSectionByIdQueryHandlerTests()
        {
            _sectionRepositoryMock = new Mock<ISectionRepository>();
            _handler = new GetSectionByIdQueryHandler(_sectionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSection_WhenSectionExists()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var query = new GetSectionByIdQuery(sectionId);
            var expectedSection = new SectionResponseDto { Id = sectionId, Title = "Section 1", Order = 1 };

            _sectionRepositoryMock
                .Setup(repo => repo.GetByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSection);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedSection);
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenSectionDoesNotExist()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var query = new GetSectionByIdQuery(sectionId);

            _sectionRepositoryMock
                .Setup(repo => repo.GetByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((SectionResponseDto?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
