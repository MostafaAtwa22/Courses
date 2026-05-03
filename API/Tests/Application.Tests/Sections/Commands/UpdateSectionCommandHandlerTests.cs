using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Section;
using Application.Features.Sections.Commands.Update;
using FluentAssertions;
using Moq;
using Xunit;
using Domain.Entities;

namespace Application.Tests.Sections.Commands
{
    public class UpdateSectionCommandHandlerTests
    {
        private readonly Mock<ISectionRepository> _sectionRepositoryMock;
        private readonly UpdateSectionCommandHandler _handler;

        public UpdateSectionCommandHandlerTests()
        {
            _sectionRepositoryMock = new Mock<ISectionRepository>();
            _handler = new UpdateSectionCommandHandler(_sectionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateSection_WhenSectionExists()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var sectionDto = new SectionUpdateDto { Title = "Updated Section", Order = 2 };
            var command = new UpdateSectionCommand(sectionId, sectionDto);
            var existingSection = new Section { Id = sectionId, Title = "Old Section", Order = 1 };

            _sectionRepositoryMock
                .Setup(repo => repo.GetEntityByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSection);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _sectionRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Section>(s => s.Title == "Updated Section" && s.Order == 2), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenSectionDoesNotExist()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var sectionDto = new SectionUpdateDto { Title = "Updated Section", Order = 2 };
            var command = new UpdateSectionCommand(sectionId, sectionDto);

            _sectionRepositoryMock
                .Setup(repo => repo.GetEntityByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Section?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _sectionRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Section>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
