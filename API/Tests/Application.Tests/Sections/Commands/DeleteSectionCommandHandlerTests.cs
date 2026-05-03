using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Sections.Commands.Delete;
using FluentAssertions;
using Moq;
using Xunit;
using Domain.Entities;

namespace Application.Tests.Sections.Commands
{
    public class DeleteSectionCommandHandlerTests
    {
        private readonly Mock<ISectionRepository> _sectionRepositoryMock;
        private readonly DeleteSectionCommandHandler _handler;

        public DeleteSectionCommandHandlerTests()
        {
            _sectionRepositoryMock = new Mock<ISectionRepository>();
            _handler = new DeleteSectionCommandHandler(_sectionRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteSection_WhenSectionExists()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var command = new DeleteSectionCommand(sectionId);
            var existingSection = new Section { Id = sectionId, Title = "Old Section", Order = 1 };

            _sectionRepositoryMock
                .Setup(repo => repo.GetEntityByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingSection);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _sectionRepositoryMock.Verify(repo => repo.DeleteAsync(sectionId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenSectionDoesNotExist()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var command = new DeleteSectionCommand(sectionId);

            _sectionRepositoryMock
                .Setup(repo => repo.GetEntityByIdAsync(sectionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Section?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _sectionRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
