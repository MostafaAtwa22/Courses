using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Course;
using Application.Features.Discount.Commands.Create;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Discount.Commands;

public class CreateDiscountCommandHandlerTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<ICourseDiscountRepository> _discountRepositoryMock;
    private readonly CreateDiscountCommandHandler _handler;

    public CreateDiscountCommandHandlerTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _discountRepositoryMock = new Mock<ICourseDiscountRepository>();

        _handler = new CreateDiscountCommandHandler(
            _courseRepositoryMock.Object,
            _discountRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDiscountId_WhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var discountId = Guid.NewGuid();
        var dto = new CreateCourseDiscountDto 
        { 
            Percentage = 20m,
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddDays(7)
        };
        var command = new CreateDiscountCommand(courseId, dto);
        var course = new Course { Id = courseId };

        _courseRepositoryMock.Setup(x => x.GetEntityByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);
        _discountRepositoryMock.Setup(x => x.AddAsync(It.IsAny<CourseDiscount>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(discountId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(discountId);
        _courseRepositoryMock.Verify(x => x.GetEntityByIdAsync(courseId, It.IsAny<CancellationToken>()), Times.Once);
        _discountRepositoryMock.Verify(x => x.AddAsync(It.IsAny<CourseDiscount>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var dto = new CreateCourseDiscountDto 
        { 
            Percentage = 20m,
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddDays(7)
        };
        var command = new CreateDiscountCommand(courseId, dto);

        _courseRepositoryMock.Setup(x => x.GetEntityByIdAsync(courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Course?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"Course\" with key \"{courseId}\" was not found.");
    }
}
