using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Authentication;
using Domain.Enums;
using Domain.Enums.Identity;
using FluentAssertions;

namespace API.Tests.Endpoints;

[Collection("Integration Tests")]
public class AuthenticationEndpointsTests
{
    private readonly HttpClient _client;

    public AuthenticationEndpointsTests(IntegrationTestFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldReturnCreated_WhenDataIsValid()
    {
        // Arrange
        var uniqueName = Guid.NewGuid().ToString().Substring(0, 8);
        var dto = new RegisterDto
        {
            Email = $"{uniqueName}@example.com",
            UserName = uniqueName,
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            FirstName = "Test",
            LastName = "User",
            Gender = Gender.Male,
            Role = Role.Student
        };

        // Act
        var response = await _client.PostAsJsonAsync("/authentication/register", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        // We assume the user exists from the previous test or seed data
        // For integration tests, we usually use a seeded user
        var dto = new LoginDto
        {
            Email = "admin@edufocus.com",
            Password = "P@ssw0rd123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/authentication/login", dto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
    }
}
