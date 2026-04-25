using System.Net;
using System.Net.Http.Json;
using Application.Common.Models;
using Application.DTOs.Course;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace API.Tests.Endpoints
{
    public class CoursesEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CoursesEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/courses");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<CoursesResponseDto>>();
            result.Should().NotBeNull();        
        }

        [Fact]
        public async Task GetById_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetAsync($"/courses/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}