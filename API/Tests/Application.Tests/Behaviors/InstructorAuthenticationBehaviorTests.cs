using Application.Behaviors;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;
using FluentAssertions;
using MediatR;
using Moq;

namespace Application.Tests.Behaviors
{
    public class InstructorAuthenticationBehaviorTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IInstructorRepository> _instructorRepositoryMock;
        private readonly Mock<RequestHandlerDelegate<object>> _nextMock;
        private readonly InstructorAuthenticationBehavior<IRequireInstructor, object> _behavior;

        public InstructorAuthenticationBehaviorTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _instructorRepositoryMock = new Mock<IInstructorRepository>();
            _nextMock = new Mock<RequestHandlerDelegate<object>>();
            _behavior = new InstructorAuthenticationBehavior<IRequireInstructor, object>(
                _currentUserServiceMock.Object,
                _instructorRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedException_WhenUserNotAuthenticated()
        {
            // Arrange
            var request = new TestInstructorRequest();
            _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new object());

            // Act
            var act = async () => await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage("You must be logged in.");
            _currentUserServiceMock.Verify(x => x.UserId, Times.Once);
            _instructorRepositoryMock.Verify(x => x.GetByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrowForbiddenException_WhenUserIsNotInstructor()
        {
            // Arrange
            var userId = "user123";
            var request = new TestInstructorRequest();
            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _instructorRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Instructor?)null);
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new object());

            // Act
            var act = async () => await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ForbiddenException>()
                .WithMessage("Only instructors can perform this action.");
            _currentUserServiceMock.Verify(x => x.UserId, Times.Once);
            _instructorRepositoryMock.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldInjectInstructorId_WhenUserIsInstructor()
        {
            // Arrange
            var userId = "user123";
            var instructorId = Guid.NewGuid();
            var instructor = new Instructor { Id = instructorId };
            var request = new TestInstructorRequest();
            var expectedResult = new object();

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _instructorRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(instructor);
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
            request.InstructorId.Should().Be(instructorId);
            _currentUserServiceMock.Verify(x => x.UserId, Times.Once);
            _instructorRepositoryMock.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotInjectInstructorId_WhenRequestDoesNotImplementIInstructorInjectable()
        {
            // Arrange
            var userId = "user123";
            var instructorId = Guid.NewGuid();
            var instructor = new Instructor { Id = instructorId };
            var request = new TestInstructorRequestWithoutInjection();
            var expectedResult = new object();

            _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
            _instructorRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(instructor);
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            // Act
            var result = await _behavior.Handle(request, _nextMock.Object, CancellationToken.None);

            // Assert
            result.Should().Be(expectedResult);
            _currentUserServiceMock.Verify(x => x.UserId, Times.Once);
            _instructorRepositoryMock.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
        }

        // Test helpers
        private class TestInstructorRequest : IRequireInstructor, IInstructorInjectable
        {
            public Guid InstructorId { get; set; }
        }

        private class TestInstructorRequestWithoutInjection : IRequireInstructor
        {
        }
    }
}
