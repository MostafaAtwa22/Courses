using Application.Common.Interfaces;
using Application.DTOs.Section;
using Application.Features.Sections.Commands.Create;
using FluentAssertions;
using Moq;
using Xunit;
using Domain.Entities;

namespace Application.Tests.Sections.Commands
{
    public class CreateSectionCommandHandlerTests
    {
        private readonly Mock<ISectionRepository> _sectionRepositoryMock;
        private readonly CreateSectionCommandHandler _handler;

        public CreateSectionCommandHandlerTests()
        {
            _sectionRepositoryMock = new Mock<ISectionRepository>();
            _handler = new CreateSectionCommandHandler(_sectionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnGuid_WhenSectionIsCreatedSuccessfully()
        {
            // Arrange
            var sectionDto = new SectionCreateDto { Title = "New Section", Order = 1, CourseId = Guid.NewGuid() };
            var command = new CreateSectionCommand(sectionDto);
            var expectedId = Guid.NewGuid();

            _sectionRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Section>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
            _sectionRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Section>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
