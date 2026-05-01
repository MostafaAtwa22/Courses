using System.Net;
using System.Net.Http.Json;
using Application.Common.Models;
using Application.DTOs.Review;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace API.Tests.Endpoints
{
    [Collection("Integration Tests")]
    public class ReviewsEndpointsTests
    {
        private readonly HttpClient _client;
        private readonly IntegrationTestFactory<Program> _factory;

        public ReviewsEndpointsTests(IntegrationTestFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetByCourse_ReturnsOk()
        {
            // Arrange
            var courseId = "c1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"; // From seed

            // Act
            var response = await _client.GetAsync($"/reviews/course/{courseId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<PaginatedResult<ReviewResponseDto>>();
            result.Should().NotBeNull();
            result!.Items.Should().NotBeEmpty();
        }

        [Fact]
        public async Task CreateReview_ShouldReturnForbidden_WhenNotEnrolled()
        {
            // Arrange
            var userId = "d3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"; // Seeded user
            var courseId = "c2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"; // Student NOT enrolled in this in seed

            _factory.CurrentUserServiceMock.Setup(u => u.UserId).Returns(userId);

            var dto = new ReviewCreateDto 
            { 
                Headline = "Testing", 
                Comment = "No enrollment", 
                Rating = 5, 
                CourseId = Guid.Parse(courseId) 
            };

            // Act
            var response = await _client.PostAsJsonAsync("/reviews", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnNoContent_WhenOwner()
        {
            // Arrange
            var userId = "d3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"; // Owner of review a1b1...
            var reviewId = "a1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1";

            _factory.CurrentUserServiceMock.Setup(u => u.UserId).Returns(userId);

            var dto = new ReviewUpdateDto 
            { 
                Headline = "Updated Headline", 
                Comment = "Updated comment", 
                Rating = 4 
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/reviews/{reviewId}", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteReview_ShouldReturnForbidden_WhenNotOwner()
        {
            // Arrange
            var userId = "d4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"; // NOT owner of review a1b1...
            var reviewId = "a1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1";

            _factory.CurrentUserServiceMock.Setup(u => u.UserId).Returns(userId);

            // Act
            var response = await _client.DeleteAsync($"/reviews/{reviewId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}
