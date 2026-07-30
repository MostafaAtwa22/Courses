using Application.DTOs.Authentication;
using Application.DTOs.Security;
using Application.Features.Security.Commands.ConfirmEmail;
using Application.Features.Security.Commands.Disable2FA;
using Application.Features.Security.Commands.Enable2FA;
using Application.Features.Security.Commands.Generate2FA;
using Application.Features.Security.Commands.VerifyTwoFactor;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using API.Endpoints;

namespace API.Tests.Endpoints
{
    public class SecurityEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public SecurityEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task ConfirmEmail_ShouldReturnOk_WithAuthResponse()
        {
            // Arrange
            var request = new ConfirmEmailDto();
            var expectedResponse = new AuthResponseDto { Token = "test-token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await SecurityEndpoints.ConfirmEmail(request, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<AuthResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedResponse);
        }

        [Fact]
        public async Task Generate2FAToken_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<Generate2FATokenCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await SecurityEndpoints.Generate2FAToken(_mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Enable2FA_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var code = "123456";
            _mediatorMock.Setup(m => m.Send(It.IsAny<Enable2FACommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await SecurityEndpoints.Enable2FA(code, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Disable2FA_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var dto = new Disable2FADto { Password = "Password123!", Code = "123456" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<Disable2FACommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await SecurityEndpoints.Disable2FA(dto, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task VerifyTwoFactor_ShouldReturnOk_WithAuthResponse()
        {
            // Arrange
            var request = new VerifyTwoFactorDto();
            var expectedResponse = new AuthResponseDto { Token = "test-token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<VerifyTwoFactorCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await SecurityEndpoints.VerifyTwoFactor(request, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<AuthResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedResponse);
        }
    }
}
