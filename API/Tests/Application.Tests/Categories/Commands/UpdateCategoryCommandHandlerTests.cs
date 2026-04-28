using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Category;
using Application.Features.Categories.Commands.Update;
using FluentAssertions;
using Moq;
using Xunit;
using Domain.Entities;

namespace Application.Tests.Categories.Commands
{
    public class UpdateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly UpdateCategoryCommandHandler _handler;

        public UpdateCategoryCommandHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _handler = new UpdateCategoryCommandHandler(_categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new CategoryUpdateDto { Name = "Updated Name" };
            var command = new UpdateCategoryCommand(id, dto);

            _categoryRepositoryMock
                .Setup(repo => repo.GetEntityByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Category?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallUpdateAsync_WhenCategoryExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new CategoryUpdateDto { Name = "Updated Name" };
            var command = new UpdateCategoryCommand(id, dto);
            var existingCategory = new Category { Id = id, Name = "Old Name" };

            _categoryRepositoryMock
                .Setup(repo => repo.GetEntityByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCategory);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _categoryRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}