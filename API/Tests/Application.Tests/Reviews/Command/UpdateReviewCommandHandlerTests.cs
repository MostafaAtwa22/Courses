using Application.Common.Interfaces;
using Application.DTOs.Review;
using Application.Features.Reviews.Commands.Update;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Application.Common.Exceptions;

namespace Application.Tests.Reviews.Command
{
    public class UpdateReviewCommandHandlerTests
    {
        private readonly Mock<IReviewRepository> _repoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly UpdateReviewCommandHandler _handler;

        public UpdateReviewCommandHandlerTests()
        {
            _repoMock = new Mock<IReviewRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new UpdateReviewCommandHandler(_repoMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateReview_WhenUserIsOwner()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var existingReview = new Review { Id = reviewId, StudentId = studentId, Headline = "Old" };

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _repoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _repoMock.Setup(r => r.GetEntityByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingReview);

            var dto = new ReviewUpdateDto { Headline = "New", Comment = "New comment", Rating = 4 };
            var command = new UpdateReviewCommand(reviewId, dto);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            existingReview.Headline.Should().Be("New");
            _repoMock.Verify(r => r.UpdateAsync(existingReview, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var existingReview = new Review { Id = reviewId, StudentId = Guid.NewGuid(), Headline = "Old" }; // Different studentId

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _repoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _repoMock.Setup(r => r.GetEntityByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingReview);

            var command = new UpdateReviewCommand(reviewId, new ReviewUpdateDto());

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }
    }
}
