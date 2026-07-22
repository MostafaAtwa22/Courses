using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.ExternalLogin.Github;
using Domain.Entities.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Authentication.Commands;

public class CreateGithubLoginCommandHandlerTests
{
    private readonly Mock<IExternalAuthService> _externalAuthServiceMock;
    private readonly Mock<ILoginPipeline> _loginPipelineMock;
    private readonly CreateGithubLoginCommandHandler _handler;

    public CreateGithubLoginCommandHandlerTests()
    {
        _externalAuthServiceMock = new Mock<IExternalAuthService>();
        _loginPipelineMock = new Mock<ILoginPipeline>();

        _handler = new CreateGithubLoginCommandHandler(
            _externalAuthServiceMock.Object,
            _loginPipelineMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenGithubLoginIsSuccessful()
    {
        // Arrange
        var dto = new GithubLoginDto { Code = "github_code", RedirectUri = "http://localhost" };
        var command = new CreateGithubLoginCommand(dto);
        var user = new ApplicationUser { Email = "test@github.com" };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt_token" };

        _externalAuthServiceMock.Setup(x => x.GithubLoginAsync(dto))
            .ReturnsAsync(user);
        _loginPipelineMock.Setup(x => x.ExecuteAsync(user))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
        _externalAuthServiceMock.Verify(x => x.GithubLoginAsync(dto), Times.Once);
        _loginPipelineMock.Verify(x => x.ExecuteAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallExternalAuthService_WithCorrectDto()
    {
        // Arrange
        var dto = new GithubLoginDto { Code = "github_code", RedirectUri = "http://localhost" };
        var command = new CreateGithubLoginCommand(dto);
        var user = new ApplicationUser { Email = "test@github.com" };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "jwt_token" };

        _externalAuthServiceMock.Setup(x => x.GithubLoginAsync(dto))
            .ReturnsAsync(user);
        _loginPipelineMock.Setup(x => x.ExecuteAsync(user))
            .ReturnsAsync(authResponse);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _externalAuthServiceMock.Verify(x => x.GithubLoginAsync(dto), Times.Once);
    }
}
