using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.ExternalLogin.Facebook;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Authentication.Commands;

public class CreateFacebookLoginCommandHandlerTests
{
    private readonly Mock<IExternalAuthService> _externalAuthServiceMock;
    private readonly Mock<ILoginPipeline> _loginPipelineMock;
    private readonly CreateFacebookLoginCommandHandler _handler;

    public CreateFacebookLoginCommandHandlerTests()
    {
        _externalAuthServiceMock = new Mock<IExternalAuthService>();
        _loginPipelineMock = new Mock<ILoginPipeline>();

        _handler = new CreateFacebookLoginCommandHandler(
            _externalAuthServiceMock.Object,
            _loginPipelineMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenFacebookLoginIsSuccessful()
    {
        // Arrange
        var dto = new FacebookLoginDto { AccessToken = "facebook_token" };
        var command = new CreateFacebookLoginCommand(dto);
        var user = new ApplicationUser { Email = "test@facebook.com" };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt_token" };

        _externalAuthServiceMock.Setup(x => x.FacebookLoginAsync(dto))
            .ReturnsAsync(user);
        _loginPipelineMock.Setup(x => x.ExecuteAsync(user))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
        _externalAuthServiceMock.Verify(x => x.FacebookLoginAsync(dto), Times.Once);
        _loginPipelineMock.Verify(x => x.ExecuteAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallExternalAuthService_WithCorrectDto()
    {
        // Arrange
        var dto = new FacebookLoginDto { AccessToken = "facebook_token" };
        var command = new CreateFacebookLoginCommand(dto);
        var user = new ApplicationUser { Email = "test@facebook.com" };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt_token" };

        _externalAuthServiceMock.Setup(x => x.FacebookLoginAsync(dto))
            .ReturnsAsync(user);
        _loginPipelineMock.Setup(x => x.ExecuteAsync(user))
            .ReturnsAsync(authResponse);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _externalAuthServiceMock.Verify(x => x.FacebookLoginAsync(dto), Times.Once);
    }
}
