using API.Endpoints;
using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.ExternalLogin.Facebook;
using Application.Features.Authentication.Commands.ExternalLogin.Google;
using Application.Features.Authentication.Commands.Login;
using Application.Features.Authentication.Commands.Register;
using Application.Features.Authentication.Commands.RefreshToken;
using Application.Features.Authentication.Commands.RevokeToken;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class AuthenticationEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<HttpContext> _httpContextMock;

        public AuthenticationEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _httpContextMock = new Mock<HttpContext>();
            var responseMock = new Mock<HttpResponse>();
            var cookiesMock = new Mock<IRequestCookieCollection>();
            var responseCookiesMock = new Mock<IResponseCookies>();

            _httpContextMock.Setup(x => x.Response).Returns(responseMock.Object);
            _httpContextMock.Setup(x => x.Request.Cookies).Returns(cookiesMock.Object);
            responseMock.Setup(x => x.Cookies).Returns(responseCookiesMock.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var request = new RegisterDto { Email = "test@example.com", Password = "Password123!" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRegisterCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await AuthenticationEndpoints.Register(request, _mediatorMock.Object);

            // Assert
            var createdResult = result.Result as Created;
            createdResult.Should().NotBeNull();
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WithAuthResponse_WhenSuccessful()
        {
            // Arrange
            var request = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            var expectedResponse = new AuthResponseDto { Token = "test-token", RefreshToken = "refresh-token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateLoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await AuthenticationEndpoints.Login(request, _mediatorMock.Object, _httpContextMock.Object);

            // Assert
            var okResult = result.Result as Ok<AuthResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(x => x.RefreshToken));
        }

        [Fact]
        public async Task GoogleLogin_ShouldReturnOk_WithAuthResponse_WhenSuccessful()
        {
            // Arrange
            var request = new GoogleLoginDto { IdToken = "google-id-token" };
            var expectedResponse = new AuthResponseDto { Token = "test-token", RefreshToken = "refresh-token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateGoogleLoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await AuthenticationEndpoints.GoogleLogin(request, _mediatorMock.Object, _httpContextMock.Object);

            // Assert
            var okResult = result.Result as Ok<AuthResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(x => x.RefreshToken));
        }

        [Fact]
        public async Task FacebookLogin_ShouldReturnOk_WithAuthResponse_WhenSuccessful()
        {
            // Arrange
            var request = new FacebookLoginDto { AccessToken = "facebook-access-token" };
            var expectedResponse = new AuthResponseDto { Token = "test-token", RefreshToken = "refresh-token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateFacebookLoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await AuthenticationEndpoints.FacebookLogin(request, _mediatorMock.Object, _httpContextMock.Object);

            // Assert
            var okResult = result.Result as Ok<AuthResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(x => x.RefreshToken));
        }

        [Fact]
        public async Task GithubLogin_ShouldReturnOk_WithAuthResponse_WhenSuccessful()
        {
            // Arrange
            var request = new GithubLoginDto { Code = "github-code" };
            var expectedResponse = new AuthResponseDto { Token = "test-token", RefreshToken = "refresh-token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<Application.Features.Authentication.Commands.ExternalLogin.Github.CreateGithubLoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await AuthenticationEndpoints.GithubLogin(request, _mediatorMock.Object, _httpContextMock.Object);

            // Assert
            var okResult = result.Result as Ok<AuthResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(x => x.RefreshToken));
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnOk_WithAuthResponse_WhenTokenIsValid()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            var expectedResponse = new AuthResponseDto { Token = "new-test-token", RefreshToken = "new-refresh-token" };
            
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.TryGetValue("refreshToken", out refreshToken)).Returns(true);
            _httpContextMock.Setup(x => x.Request.Cookies).Returns(cookiesMock.Object);
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await AuthenticationEndpoints.RefreshToken(_httpContextMock.Object, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<AuthResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResponse, options => options.Excluding(x => x.RefreshToken));
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            // Arrange
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.TryGetValue("refreshToken", out It.Ref<string>.IsAny!)).Returns(false);
            _httpContextMock.Setup(x => x.Request.Cookies).Returns(cookiesMock.Object);

            // Act
            var result = await AuthenticationEndpoints.RefreshToken(_httpContextMock.Object, _mediatorMock.Object);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedHttpResult;
            unauthorizedResult.Should().NotBeNull();
        }

        [Fact]
        public async Task RevokeToken_ShouldReturnOk_WhenTokenIsValid()
        {
            // Arrange
            var refreshToken = "valid-refresh-token";
            
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.TryGetValue("refreshToken", out refreshToken)).Returns(true);
            _httpContextMock.Setup(x => x.Request.Cookies).Returns(cookiesMock.Object);
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateRevokeTokenCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await AuthenticationEndpoints.RevokeToken(_httpContextMock.Object, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok;
            okResult.Should().NotBeNull();
        }

        [Fact]
        public async Task RevokeToken_ShouldReturnUnauthorized_WhenTokenIsMissing()
        {
            // Arrange
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c.TryGetValue("refreshToken", out It.Ref<string>.IsAny!)).Returns(false);
            _httpContextMock.Setup(x => x.Request.Cookies).Returns(cookiesMock.Object);

            // Act
            var result = await AuthenticationEndpoints.RevokeToken(_httpContextMock.Object, _mediatorMock.Object);

            // Assert
            var unauthorizedResult = result.Result as UnauthorizedHttpResult;
            unauthorizedResult.Should().NotBeNull();
        }
    }
}
