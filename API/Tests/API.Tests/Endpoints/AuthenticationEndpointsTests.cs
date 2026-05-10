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
}
