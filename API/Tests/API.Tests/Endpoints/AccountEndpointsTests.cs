using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Models;
using Application.DTOs.Account;
using FluentAssertions;

namespace API.Tests.Endpoints;

[Collection("Integration Tests")]
public class AccountEndpointsTests
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _options;

    public AccountEndpointsTests(IntegrationTestFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    [Fact]
    public async Task GetUsers_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/account/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<UserResponseDto>>(_options);
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserById_ShouldReturnNotFound_WhenIdDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"/account/users/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task ForgetPassword_ShouldReturnNoContent()
    {
        // Arrange
        var dto = new ForgetPasswordDto { Email = "admin@edufocus.com" };

        // Act
        var response = await _client.PostAsJsonAsync("/account/forget-password", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnBadRequest_WhenTokenIsInvalid()
    {
        // Arrange
        var dto = new ResetPasswordDto 
        { 
            Email = "admin@edufocus.com", 
            Token = "invalid", 
            NewPassword = "P@ssw0rd123!",
            ConfirmNewPassword = "P@ssw0rd123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/account/reset-password", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
