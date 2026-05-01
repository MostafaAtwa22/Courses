using Application.Common.Interfaces;
using Application.Features.Reviews.Commands.Delete;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Application.Common.Exceptions;

namespace Application.Tests.Reviews.Command
{
    public class DeleteReviewCommandHandlerTests
    {
        private readonly Mock<IReviewRepository> _repoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly DeleteReviewCommandHandler _handler;

        public DeleteReviewCommandHandlerTests()
        {
            _repoMock = new Mock<IReviewRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new DeleteReviewCommandHandler(_repoMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteReview_WhenUserIsOwner()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var existingReview = new Review { Id = reviewId, StudentId = studentId };

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _repoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _repoMock.Setup(r => r.GetEntityByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingReview);

            var command = new DeleteReviewCommand(reviewId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repoMock.Verify(r => r.DeleteAsync(reviewId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenUserIsNotOwner()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();
            var existingReview = new Review { Id = reviewId, StudentId = Guid.NewGuid() };

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _repoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _repoMock.Setup(r => r.GetEntityByIdAsync(reviewId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingReview);

            var command = new DeleteReviewCommand(reviewId);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }
    }
}
