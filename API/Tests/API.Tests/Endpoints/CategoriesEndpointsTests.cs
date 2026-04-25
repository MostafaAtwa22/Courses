using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Application.Common.Models;
using Application.DTOs.Category;

namespace API.Tests.Endpoints
{
    public class CategoriesEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CategoriesEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/categories");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<CategoryResponseDto>>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound()
        {
            // Arrange 
            var id = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetAsync($"/categories/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            // Arrange
            var uniqueName = $"Category_{Guid.NewGuid().ToString().Substring(0, 8)}";
            var newCategory = new CategoryCreateDto
            {
                Name = uniqueName,
                Slug = uniqueName.ToLower()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/categories", newCategory);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CategoryResponseDto>();
            result.Should().NotBeNull();
            result!.Id.Should().NotBeEmpty();
            result.Name.Should().Be(uniqueName);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidCategory = new CategoryCreateDto
            {
                Name = "", // Invalid name (too short/empty)
                Slug = "invalid-category"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/categories", invalidCategory);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Lifecycle_ShouldWorkCorrectly()
        {
            // 1. Create
            var uniqueName = $"Lifecycle_{Guid.NewGuid().ToString().Substring(0, 8)}";
            var createDto = new CategoryCreateDto { Name = uniqueName, Slug = uniqueName.ToLower() };
            var createResponse = await _client.PostAsJsonAsync("/categories", createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdCategory = await createResponse.Content.ReadFromJsonAsync<CategoryResponseDto>();
            var id = createdCategory!.Id;

            // 2. Get By Id
            var getResponse = await _client.GetAsync($"/categories/{id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadFromJsonAsync<CategoryResponseDto>();
            getResult!.Name.Should().Be(uniqueName);

            // 3. Update
            var updateDto = new CategoryUpdateDto { Name = uniqueName + " Updated", Slug = uniqueName.ToLower() + "-updated" };
            var updateResponse = await _client.PutAsJsonAsync($"/categories/{id}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 4. Verify Update
            var getResponseAfterUpdate = await _client.GetAsync($"/categories/{id}");
            var updatedResult = await getResponseAfterUpdate.Content.ReadFromJsonAsync<CategoryResponseDto>();
            updatedResult!.Name.Should().Be(uniqueName + " Updated");

            // 5. Delete
            var deleteResponse = await _client.DeleteAsync($"/categories/{id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 6. Verify Delete
            var getResponseAfterDelete = await _client.GetAsync($"/categories/{id}");
            getResponseAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}