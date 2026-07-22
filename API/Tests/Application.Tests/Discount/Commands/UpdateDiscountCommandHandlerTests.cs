using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Course;
using Application.Features.Discount.Commands.Update;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Discount.Commands;

public class UpdateDiscountCommandHandlerTests
{
    private readonly Mock<ICourseDiscountRepository> _discountRepositoryMock;
    private readonly UpdateDiscountCommandHandler _handler;

    public UpdateDiscountCommandHandlerTests()
    {
        _discountRepositoryMock = new Mock<ICourseDiscountRepository>();
        _handler = new UpdateDiscountCommandHandler(_discountRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDiscount_WhenDiscountExists()
    {
        // Arrange
        var discountId = Guid.NewGuid();
        var dto = new UpdateCourseDiscountDto 
        { 
            Percentage = 30m,
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddDays(14)
        };
        var command = new UpdateDiscountCommand(discountId, dto);
        var discount = new CourseDiscount { Id = discountId };

        _discountRepositoryMock.Setup(x => x.GetEntityByIdAsync(discountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discount);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _discountRepositoryMock.Verify(x => x.UpdateAsync(discount, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenDiscountDoesNotExist()
    {
        // Arrange
        var discountId = Guid.NewGuid();
        var dto = new UpdateCourseDiscountDto 
        { 
            Percentage = 30m,
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddDays(14)
        };
        var command = new UpdateDiscountCommand(discountId, dto);

        _discountRepositoryMock.Setup(x => x.GetEntityByIdAsync(discountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CourseDiscount?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"CourseDiscount\" with key \"{discountId}\" was not found.");
    }
}
