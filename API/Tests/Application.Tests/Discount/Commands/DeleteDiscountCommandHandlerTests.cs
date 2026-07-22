using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Discount.Commands.Delete;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Discount.Commands;

public class DeleteDiscountCommandHandlerTests
{
    private readonly Mock<ICourseDiscountRepository> _discountRepositoryMock;
    private readonly DeleteDiscountCommandHandler _handler;

    public DeleteDiscountCommandHandlerTests()
    {
        _discountRepositoryMock = new Mock<ICourseDiscountRepository>();
        _handler = new DeleteDiscountCommandHandler(_discountRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteDiscount_WhenDiscountExists()
    {
        // Arrange
        var discountId = Guid.NewGuid();
        var command = new DeleteDiscountCommand(discountId);
        var discount = new CourseDiscount { Id = discountId };

        _discountRepositoryMock.Setup(x => x.GetEntityByIdAsync(discountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discount);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _discountRepositoryMock.Verify(x => x.DeleteAsync(discountId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenDiscountDoesNotExist()
    {
        // Arrange
        var discountId = Guid.NewGuid();
        var command = new DeleteDiscountCommand(discountId);

        _discountRepositoryMock.Setup(x => x.GetEntityByIdAsync(discountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseDiscount?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"CourseDiscount\" with key \"{discountId}\" was not found.");
    }
}
