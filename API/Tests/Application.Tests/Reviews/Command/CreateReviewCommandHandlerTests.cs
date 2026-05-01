using Application.Common.Interfaces;
using Application.DTOs.Review;
using Application.Features.Reviews.Commands.Create;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Application.Common.Exceptions;

namespace Application.Tests.Reviews.Command
{
    public class CreateReviewCommandHandlerTests
    {
        private readonly Mock<IReviewRepository> _repoMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateReviewCommandHandler _handler;

        public CreateReviewCommandHandlerTests()
        {
            _repoMock = new Mock<IReviewRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _handler = new CreateReviewCommandHandler(_repoMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnGuid_WhenEnrollmentExistsAndNoPreviousReview()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedReviewId = Guid.NewGuid();

            var dto = new ReviewCreateDto 
            { 
                Headline = "Great!", 
                Comment = "Loved it", 
                Rating = 5, 
                CourseId = courseId 
            };
            var command = new CreateReviewCommand(dto);

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _repoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _repoMock.Setup(r => r.IsStudentEnrolledAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _repoMock.Setup(r => r.HasStudentReviewedAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Review>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedReviewId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(expectedReviewId);
            _repoMock.Verify(r => r.CreateAsync(It.Is<Review>(x => x.StudentId == studentId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorized_WhenUserNotLoggedIn()
        {
            // Arrange
            _currentUserServiceMock.Setup(u => u.UserId).Returns((string?)null);
            var command = new CreateReviewCommand(new ReviewCreateDto());

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowForbidden_WhenStudentNotEnrolled()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _repoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _repoMock.Setup(r => r.IsStudentEnrolledAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new CreateReviewCommand(new ReviewCreateDto { CourseId = courseId });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>();
        }

        [Fact]
        public async Task Handle_ShouldThrowConflict_WhenStudentAlreadyReviewed()
        {
            // Arrange
            var userId = "user-123";
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _currentUserServiceMock.Setup(u => u.UserId).Returns(userId);
            _repoMock.Setup(r => r.GetStudentIdByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(studentId);
            _repoMock.Setup(r => r.IsStudentEnrolledAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _repoMock.Setup(r => r.HasStudentReviewedAsync(studentId, courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var command = new CreateReviewCommand(new ReviewCreateDto { CourseId = courseId });

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>();
        }
    }
}
