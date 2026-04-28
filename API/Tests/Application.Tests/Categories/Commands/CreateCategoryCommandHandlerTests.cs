using Application.Common.Interfaces;
using Application.DTOs.Category;
using Application.Features.Categories.Commands.Create;
using FluentAssertions;
using Moq;
using Xunit;
using Domain.Entities;

namespace Application.Tests.Categories.Commands
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _handler = new CreateCategoryCommandHandler(_categoryRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnGuid_WhenCategoryIsCreatedSuccessfully()
        {
            // Arrange
            var categoryDto = new CategoryCreateDto { Name = "New Category" };
            var command = new CreateCategoryCommand(categoryDto);
            var expectedId = Guid.NewGuid();

            _categoryRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedId);
            _categoryRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}