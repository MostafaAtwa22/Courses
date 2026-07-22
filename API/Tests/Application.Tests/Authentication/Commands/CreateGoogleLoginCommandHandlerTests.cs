using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.ExternalLogin.Google;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Authentication.Commands;

public class CreateGoogleLoginCommandHandlerTests
{
    private readonly Mock<IExternalAuthService> _externalAuthServiceMock;
    private readonly Mock<ILoginPipeline> _loginPipelineMock;
    private readonly CreateGoogleLoginCommandHandler _handler;

    public CreateGoogleLoginCommandHandlerTests()
    {
        _externalAuthServiceMock = new Mock<IExternalAuthService>();
        _loginPipelineMock = new Mock<ILoginPipeline>();

        _handler = new CreateGoogleLoginCommandHandler(
            _externalAuthServiceMock.Object,
            _loginPipelineMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenGoogleLoginIsSuccessful()
    {
        // Arrange
        var dto = new GoogleLoginDto { IdToken = "google_token" };
        var command = new CreateGoogleLoginCommand(dto);
        var user = new ApplicationUser { Email = "test@gmail.com" };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt_token" };

        _externalAuthServiceMock.Setup(x => x.GoogleLoginAsync(dto))
            .ReturnsAsync(user);
        _loginPipelineMock.Setup(x => x.ExecuteAsync(user))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
        _externalAuthServiceMock.Verify(x => x.GoogleLoginAsync(dto), Times.Once);
        _loginPipelineMock.Verify(x => x.ExecuteAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallExternalAuthService_WithCorrectDto()
    {
        // Arrange
        var dto = new GoogleLoginDto { IdToken = "google_token" };
        var command = new CreateGoogleLoginCommand(dto);
        var user = new ApplicationUser { Email = "test@gmail.com" };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt_token" };

        _externalAuthServiceMock.Setup(x => x.GoogleLoginAsync(dto))
            .ReturnsAsync(user);
        _loginPipelineMock.Setup(x => x.ExecuteAsync(user))
            .ReturnsAsync(authResponse);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _externalAuthServiceMock.Verify(x => x.GoogleLoginAsync(dto), Times.Once);
    }
}
