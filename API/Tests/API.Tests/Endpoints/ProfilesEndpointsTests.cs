using API.Endpoints;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.ChangePassword;
using Application.Features.Profiles.Commands.Delete;
using Application.Features.Profiles.Commands.DeleteImage;
using Application.Features.Profiles.Commands.SetPassword;
using Application.Features.Profiles.Commands.Update;
using Application.Features.Profiles.Commands.UpdateImage;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class ProfilesEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public ProfilesEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new UpdateProfileDto { FirstName = "John", LastName = "Doe" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProfileCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProfilesEndpoints.UpdateProfile(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteProfile_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new DeleteProfileDto { Password = "Password123!" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProfileCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProfilesEndpoints.DeleteProfile(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateProfileImage_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new UpdateProfileImageDto { Image = null! };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateProfileImageCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProfilesEndpoints.UpdateProfileImage(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteProfileImage_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteProfileImageCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProfilesEndpoints.DeleteProfileImage(_mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new ChangePasswordDto { OldPassword = "OldPassword123!", NewPassword = "NewPassword123!", ConfirmNewPassword = "NewPassword123!" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProfilesEndpoints.ChangePassword(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task SetPassword_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new SetPasswordDto { NewPassword = "NewPassword123!" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<SetPasswordCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ProfilesEndpoints.SetPassword(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
