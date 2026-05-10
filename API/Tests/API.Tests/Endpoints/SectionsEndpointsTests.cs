using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Application.Common.Models;
using Application.DTOs.Section;

namespace API.Tests.Endpoints
{
    [Collection("Integration Tests")]
    public class SectionsEndpointsTests
    {
        private readonly HttpClient _client;

        public SectionsEndpointsTests(IntegrationTestFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/sections");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };
            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<SectionResponseDto>>(options);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound()
        {
            // Arrange 
            var id = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetAsync($"/sections/{id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetByCourseId_ShouldReturnOk()
        {
            // Arrange 
            var courseId = Guid.NewGuid().ToString();

            // Act
            var response = await _client.GetAsync($"/sections/course/{courseId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };
            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<SectionResponseDto>>(options);
            result.Should().NotBeNull();
        }

        private async Task<Guid> CreateValidCourseAsync()
        {
            // Category setup
            var uniqueCatName = $"Category_{Guid.NewGuid().ToString().Substring(0, 8)}";
            var catReq = new Application.DTOs.Category.CategoryCreateDto { Name = uniqueCatName, Slug = uniqueCatName.ToLower() };
            var catRes = await _client.PostAsJsonAsync("/categories", catReq);
            var catObj = await catRes.Content.ReadFromJsonAsync<Application.DTOs.Category.CategoryResponseDto>();
            var validCatId = catObj!.Id;

            // Course setup
            var title = $"Course_{Guid.NewGuid().ToString().Substring(0, 8)}";
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(title), "Title");
            content.Add(new StringContent("This is a valid long description that is more than 50 characters long so it passes the validation process."), "Description");
            content.Add(new StringContent(Domain.Enums.CourseStatus.InProgress.ToString()), "Status");
            content.Add(new StringContent("150"), "Cost");
            content.Add(new StringContent(validCatId.ToString()), "CategoryId");
            content.Add(new StringContent("f1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), "InstructorId");
            content.Add(new StringContent("English"), "Language");

            var fileContent = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 });
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "PictureUrl", "test.jpg");

            var videoContent = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 });
            videoContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4");
            content.Add(videoContent, "IntroVideo", "test.mp4");

            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };
            var courseRes = await _client.PostAsync("/courses", content);
            var courseObj = await courseRes.Content.ReadFromJsonAsync<Application.DTOs.Course.CourseResponseDto>(options);
            return courseObj!.Id;
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var validCourseId = await CreateValidCourseAsync();

            // Arrange
            var newSection = new SectionCreateDto
            {
                Title = "Test Section",
                Order = 1,
                CourseId = validCourseId
            };

            // Act
            var response = await _client.PostAsJsonAsync("/sections", newSection);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };
            var result = await response.Content.ReadFromJsonAsync<SectionResponseDto>(options);
            result.Should().NotBeNull();
            result!.Id.Should().NotBeEmpty();
            result.Title.Should().Be("Test Section");
            result.Order.Should().Be(1);
        }

        [Fact]
        public async Task Create_ShouldReturnBadRequest_WhenInvalid()
        {
            // Arrange
            var invalidSection = new SectionCreateDto
            {
                Title = "", // Invalid title
                Order = 1,
                CourseId = Guid.Empty
            };

            // Act
            var response = await _client.PostAsJsonAsync("/sections", invalidSection);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Lifecycle_ShouldWorkCorrectly()
        {
            var validCourseId = await CreateValidCourseAsync();

            // 1. Create
            var createDto = new SectionCreateDto { Title = "Lifecycle Section", Order = 1, CourseId = validCourseId };
            var createResponse = await _client.PostAsJsonAsync("/sections", createDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } };
            var createdSection = await createResponse.Content.ReadFromJsonAsync<SectionResponseDto>(options);
            var id = createdSection!.Id;

            // 2. Get By Id
            var getResponse = await _client.GetAsync($"/sections/{id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadFromJsonAsync<SectionResponseDto>(options);
            getResult!.Title.Should().Be("Lifecycle Section");

            // 3. Update
            var updateDto = new SectionUpdateDto { Title = "Lifecycle Section Updated", Order = 2 };
            var updateResponse = await _client.PutAsJsonAsync($"/sections/{id}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 4. Verify Update
            var getResponseAfterUpdate = await _client.GetAsync($"/sections/{id}");
            var updatedResult = await getResponseAfterUpdate.Content.ReadFromJsonAsync<SectionResponseDto>(options);
            updatedResult!.Title.Should().Be("Lifecycle Section Updated");
            updatedResult!.Order.Should().Be(2);

            // 5. Delete
            var deleteResponse = await _client.DeleteAsync($"/sections/{id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 6. Verify Delete
            var getResponseAfterDelete = await _client.GetAsync($"/sections/{id}");
            getResponseAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
