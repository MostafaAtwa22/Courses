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

            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<CourseResponseDto>>();
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
        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            // Category setup
            var uniqueCatName = $"Category_{Guid.NewGuid().ToString().Substring(0, 8)}";
            var catReq = new Application.DTOs.Category.CategoryCreateDto { Name = uniqueCatName, Slug = uniqueCatName.ToLower() };
            var catRes = await _client.PostAsJsonAsync("/categories", catReq);
            var catObj = await catRes.Content.ReadFromJsonAsync<Application.DTOs.Category.CategoryResponseDto>();
            var validCatId = catObj!.Id;

            // Arrange
            var title = $"Course_{Guid.NewGuid().ToString().Substring(0, 8)}";
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(title), "Title");
            content.Add(new StringContent("This is a valid long description that is more than 50 characters long so it passes the validation process."), "Description");
            content.Add(new StringContent(Domain.Enums.CourseStatus.InProgress.ToString()), "Status");
            content.Add(new StringContent("150"), "Cost");
            content.Add(new StringContent(validCatId.ToString()), "CategoryId");

            var fileContent = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 });
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "PictureUrl", "test.jpg");

            // Act
            var response = await _client.PostAsync("/courses", content);

            // Assert
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"500 Error: {error}");
            }
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<CourseResponseDto>();
            result.Should().NotBeNull();
            result!.Id.Should().NotBeEmpty();
            result.Title.Should().Be(title);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest()
        {
            // Arrange
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(""), "Title");
            content.Add(new StringContent("Too short"), "Description");
            // Missing other fields or invalid fields

            // Act
            var response = await _client.PostAsync("/courses", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Lifecycle_ShouldWorkCorrectly()
        {
            // Category setup
            var uniqueCatName = $"Category_{Guid.NewGuid().ToString().Substring(0, 8)}";
            var catReq = new Application.DTOs.Category.CategoryCreateDto { Name = uniqueCatName, Slug = uniqueCatName.ToLower() };
            var catRes = await _client.PostAsJsonAsync("/categories", catReq);
            var catObj = await catRes.Content.ReadFromJsonAsync<Application.DTOs.Category.CategoryResponseDto>();
            var validCatId = catObj!.Id;
            
            // 1. Create
            var title = $"Lifecycle_Course_{Guid.NewGuid().ToString().Substring(0, 8)}";
            using var createContent = new MultipartFormDataContent();
            createContent.Add(new StringContent(title), "Title");
            createContent.Add(new StringContent("This is a valid long description that is more than 50 characters long so it passes the validation process."), "Description");
            createContent.Add(new StringContent(Domain.Enums.CourseStatus.InProgress.ToString()), "Status");
            createContent.Add(new StringContent("200"), "Cost");
            createContent.Add(new StringContent(validCatId.ToString()), "CategoryId");
            var imageContent = new ByteArrayContent(new byte[] { 0x01 });
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            createContent.Add(imageContent, "PictureUrl", "lifecycle.jpg");

            var createResponse = await _client.PostAsync("/courses", createContent);
            
            if(createResponse.StatusCode == HttpStatusCode.Created) 
            {
                var createdCourse = await createResponse.Content.ReadFromJsonAsync<CourseResponseDto>();
                var id = createdCourse!.Id;

                // 2. Get By Id
                var getResponse = await _client.GetAsync($"/courses/{id}");
                getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                var getResult = await getResponse.Content.ReadFromJsonAsync<CourseResponseDto>();
                getResult!.Title.Should().Be(title);

                // 3. Update
                using var updateContent = new MultipartFormDataContent();
                updateContent.Add(new StringContent(title + " Updated"), "Title");
                updateContent.Add(new StringContent("This is a valid long description that is more than 50 characters long so it passes the validation process."), "Description");
                updateContent.Add(new StringContent(Domain.Enums.CourseStatus.Done.ToString()), "Status");
                updateContent.Add(new StringContent("250"), "Cost");
                updateContent.Add(new StringContent(validCatId.ToString()), "CategoryId");
                var updateImage = new ByteArrayContent(new byte[] { 0x02 });
                updateImage.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                updateContent.Add(updateImage, "PictureUrl", "updated.jpg");

                var updateResponse = await _client.PutAsync($"/courses/{id}", updateContent);
                updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

                // 4. Verify Update
                var getResponseAfterUpdate = await _client.GetAsync($"/courses/{id}");
                var updatedResult = await getResponseAfterUpdate.Content.ReadFromJsonAsync<CourseResponseDto>();
                updatedResult!.Title.Should().Be(title + " Updated");
                updatedResult.Status.Should().Be(Domain.Enums.CourseStatus.Done);

                // 5. Delete
                var deleteResponse = await _client.DeleteAsync($"/courses/{id}");
                deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

                // 6. Verify Delete
                var getResponseAfterDelete = await _client.GetAsync($"/courses/{id}");
                getResponseAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }
    }
}