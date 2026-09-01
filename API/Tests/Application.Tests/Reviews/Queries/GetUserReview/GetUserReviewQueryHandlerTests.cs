using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs.Review;
using Application.Features.Reviews.Queries.GetUserReview;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Reviews.Queries.GetUserReview;

public class GetUserReviewQueryHandlerTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly GetUserReviewQueryHandler _handler;

    public GetUserReviewQueryHandlerTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _handler = new GetUserReviewQueryHandler(_reviewRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnReviewDto_WhenReviewExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var studentId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var query = new GetUserReviewQuery(courseId) { User = user };
        var expectedDto = new ReviewResponseDto
        {
            Id = Guid.NewGuid(),
            Headline = "Great course!",
            Rating = 5
        };

        _reviewRepositoryMock.Setup(x => x.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentId);
        _reviewRepositoryMock.Setup(x => x.GetByUserAndCourseAsync(studentId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        _reviewRepositoryMock.Verify(x => x.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _reviewRepositoryMock.Verify(x => x.GetByUserAndCourseAsync(studentId, courseId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenReviewDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var studentId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var query = new GetUserReviewQuery(courseId) { User = user };

        _reviewRepositoryMock.Setup(x => x.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentId);
        _reviewRepositoryMock.Setup(x => x.GetByUserAndCourseAsync(studentId, courseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ReviewResponseDto?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _reviewRepositoryMock.Verify(x => x.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _reviewRepositoryMock.Verify(x => x.GetByUserAndCourseAsync(studentId, courseId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenStudentIdNotFound()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var user = new ApplicationUser { Id = userId };
        var query = new GetUserReviewQuery(courseId) { User = user };

        _reviewRepositoryMock.Setup(x => x.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        // Act
        var act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"*Student*");
        _reviewRepositoryMock.Verify(x => x.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
