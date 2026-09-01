using Application.Behaviors;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.Features.Contents.Queries.GetBySection;
using FluentAssertions;
using MediatR;
using Moq;

namespace Application.Tests.Behaviors
{
    public class EnrollmentAuthorizationBehaviorTests
    {
        private readonly Mock<IContentAccessService> _contentAccessServiceMock;
        private readonly Mock<IContentRepository> _contentRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<RequestHandlerDelegate<object>> _nextMock;
        private readonly EnrollmentAuthorizationBehavior<IRequireEnrollment, object> _behavior;

        public EnrollmentAuthorizationBehaviorTests()
        {
            _contentAccessServiceMock = new Mock<IContentAccessService>();
            _contentRepositoryMock = new Mock<IContentRepository>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _nextMock = new Mock<RequestHandlerDelegate<object>>();
            _behavior = new EnrollmentAuthorizationBehavior<IRequireEnrollment, object>(
                _contentAccessServiceMock.Object,
                _contentRepositoryMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldAllowGuestAccess_WhenAllowPreviewTrue()
        {
            var courseId = Guid.NewGuid();
            var requestMock = new Mock<IRequireEnrollment>();
            requestMock.Setup(x => x.CourseId).Returns(courseId);
            requestMock.Setup(x => x.ContentId).Returns(Guid.Empty);
            requestMock.Setup(x => x.AllowPreview).Returns(true);
            
            _currentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new object());

            var result = await _behavior.Handle(requestMock.Object, _nextMock.Object, CancellationToken.None);

            result.Should().NotBeNull();
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAllowAuthenticatedUserAccess_WhenAllowPreviewTrueAndNotEnrolled()
        {
            var courseId = Guid.NewGuid();
            var requestMock = new Mock<IRequireEnrollment>();
            requestMock.Setup(x => x.CourseId).Returns(courseId);
            requestMock.Setup(x => x.ContentId).Returns(Guid.Empty);
            requestMock.Setup(x => x.AllowPreview).Returns(true);
            
            _currentUserServiceMock.Setup(x => x.UserId).Returns("user123");
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new object());

            var result = await _behavior.Handle(requestMock.Object, _nextMock.Object, CancellationToken.None);

            result.Should().NotBeNull();
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAllowEnrolledUserAccess_WhenAllowPreviewTrue()
        {
            var courseId = Guid.NewGuid();
            var requestMock = new Mock<IRequireEnrollment>();
            requestMock.Setup(x => x.CourseId).Returns(courseId);
            requestMock.Setup(x => x.ContentId).Returns(Guid.Empty);
            requestMock.Setup(x => x.AllowPreview).Returns(true);
            
            _currentUserServiceMock.Setup(x => x.UserId).Returns("user123");
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new object());

            var result = await _behavior.Handle(requestMock.Object, _nextMock.Object, CancellationToken.None);

            result.Should().NotBeNull();
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldBlockUnenrolledUser_WhenAllowPreviewFalse()
        {
            var courseId = Guid.NewGuid();
            var requestMock = new Mock<IRequireEnrollment>();
            requestMock.Setup(x => x.CourseId).Returns(courseId);
            requestMock.Setup(x => x.ContentId).Returns(Guid.Empty);
            requestMock.Setup(x => x.AllowPreview).Returns(false);
            
            _currentUserServiceMock.Setup(x => x.UserId).Returns("user123");
            _contentAccessServiceMock.Setup(x => x.HasFullCourseContentAccessAsync(courseId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var act = async () => await _behavior.Handle(requestMock.Object, _nextMock.Object, CancellationToken.None);

            await act.Should().ThrowAsync<ForbiddenException>()
                .WithMessage("You must be enrolled in this course to access this content.");
        }

        [Fact]
        public async Task Handle_ShouldAllowAccess_WhenCourseIdEmpty()
        {
            var requestMock = new Mock<IRequireEnrollment>();
            requestMock.Setup(x => x.CourseId).Returns(Guid.Empty);
            requestMock.Setup(x => x.ContentId).Returns(Guid.Empty);
            requestMock.Setup(x => x.AllowPreview).Returns(true);
            
            _currentUserServiceMock.Setup(x => x.UserId).Returns("user123");
            _nextMock.Setup(x => x(It.IsAny<CancellationToken>())).ReturnsAsync(new object());

            var result = await _behavior.Handle(requestMock.Object, _nextMock.Object, CancellationToken.None);

            result.Should().NotBeNull();
            _nextMock.Verify(x => x(It.IsAny<CancellationToken>()), Times.Once);
            _contentAccessServiceMock.Verify(x => x.HasFullCourseContentAccessAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
