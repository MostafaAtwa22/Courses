using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Profile;
using FluentAssertions;
using Moq;

namespace API.Tests.Endpoints;

[Collection("Integration Tests")]
public class ProfilesEndpointsTests
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFactory<Program> _factory;

    public ProfilesEndpointsTests(IntegrationTestFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ChangePassword_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        _factory.CurrentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var dto = new ChangePasswordDto { OldPassword = "Old", NewPassword = "New123!", ConfirmNewPassword = "New123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/profiles/change-password", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SetPassword_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn()
    {
        // Arrange
        _factory.CurrentUserServiceMock.Setup(x => x.UserId).Returns((string?)null);
        var dto = new SetPasswordDto { NewPassword = "New123!", ConfirmNewPassword = "New123!" };

        // Act
        var response = await _client.PostAsJsonAsync("/profiles/set-password", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
