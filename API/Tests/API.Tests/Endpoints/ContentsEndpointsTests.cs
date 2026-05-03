using System.Net;
using System.Net.Http.Json;
using Application.Common.Models;
using Application.DTOs.Content;
using Application.DTOs.Section;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace API.Tests.Endpoints
{
    [Collection("Integration Tests")]
    public class ContentsEndpointsTests
    {
        private readonly HttpClient _client;

        public ContentsEndpointsTests(IntegrationTestFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<Guid> CreateValidSectionAsync(Guid courseId)
        {
            var newSection = new SectionCreateDto
            {
                Title = $"Section_{Guid.NewGuid().ToString().Substring(0, 8)}",
                Order = 1,
                CourseId = courseId
            };
            var response = await _client.PostAsJsonAsync("/sections", newSection);
            var result = await response.Content.ReadFromJsonAsync<SectionResponseDto>();
            return result!.Id;
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

            var fileContent = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 });
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Add(fileContent, "PictureUrl", "test.jpg");

            var courseRes = await _client.PostAsync("/courses", content);
            var courseObj = await courseRes.Content.ReadFromJsonAsync<Application.DTOs.Course.CourseResponseDto>();
            return courseObj!.Id;
        }

        [Fact]
        public async Task Lifecycle_ShouldWorkCorrectly()
        {
            var courseId = await CreateValidCourseAsync();
            var sectionId = await CreateValidSectionAsync(courseId);

            // 1. Create
            using var createContent = new MultipartFormDataContent();
            createContent.Add(new StringContent("Lifecycle Video"), "Title");
            createContent.Add(new StringContent(ContentType.Video.ToString()), "Type");
            createContent.Add(new StringContent(sectionId.ToString()), "SectionId");
            createContent.Add(new StringContent("1"), "Order");
            createContent.Add(new StringContent("true"), "IsPreview");
            
            var videoContent = new ByteArrayContent(new byte[] { 0x00, 0x00 });
            videoContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4");
            createContent.Add(videoContent, "File", "video.mp4");

            var createResponse = await _client.PostAsync("/contents", createContent);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var createdContent = await createResponse.Content.ReadFromJsonAsync<ContentResponseDto>();
            var id = createdContent!.Id;

            // 2. Get By Id
            var getResponse = await _client.GetAsync($"/contents/{id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadFromJsonAsync<ContentResponseDto>();
            getResult!.Title.Should().Be("Lifecycle Video");

            // 3. Get By Section
            var getListResponse = await _client.GetAsync($"/contents/section/{sectionId}");
            getListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var listResult = await getListResponse.Content.ReadFromJsonAsync<PaginatedResult<ContentResponseDto>>();
            listResult!.Items.Should().Contain(x => x.Id == id);

            // 3.1 Get By Course
            var getCourseListResponse = await _client.GetAsync($"/contents/course/{courseId}");
            getCourseListResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var courseListResult = await getCourseListResponse.Content.ReadFromJsonAsync<PaginatedResult<ContentResponseDto>>();
            courseListResult!.Items.Should().Contain(x => x.Id == id);

            // 4. Update
            using var updateContent = new MultipartFormDataContent();
            updateContent.Add(new StringContent("Lifecycle Video Updated"), "Title");
            updateContent.Add(new StringContent(ContentType.Video.ToString()), "Type");
            updateContent.Add(new StringContent(sectionId.ToString()), "SectionId");
            updateContent.Add(new StringContent("2"), "Order");
            updateContent.Add(new StringContent("false"), "IsPreview");

            var updateResponse = await _client.PutAsync($"/contents/{id}", updateContent);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 5. Verify Update
            var getResponseAfterUpdate = await _client.GetAsync($"/contents/{id}");
            var updatedResult = await getResponseAfterUpdate.Content.ReadFromJsonAsync<ContentResponseDto>();
            updatedResult!.Title.Should().Be("Lifecycle Video Updated");
            updatedResult.Order.Should().Be(2);

            // 6. Delete
            var deleteResponse = await _client.DeleteAsync($"/contents/{id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 7. Verify Delete
            var getResponseAfterDelete = await _client.GetAsync($"/contents/{id}");
            getResponseAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
