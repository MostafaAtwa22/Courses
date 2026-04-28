using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Course;
using Application.Features.Categories.Queries.GetAll;
using Application.Features.Courses.Queries.GetAll;
using Domain.Enums;
using FluentAssertions;
using Moq;

namespace Application.Tests.Courses.Query
{
    public class GetCoursesHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly GetCoursesQueryHandler _handler;

        public GetCoursesHandlerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _handler = new GetCoursesQueryHandler(_courseRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedResults_WhenCalledWithValidParams()
        {
            // Arrange
            var queryParams = new QueryParams
            {
                PageNumber = 1,
                PageSize = 10
            };

            var expected = new PaginatedResult<CourseResponseDto>
            {
                Items = new List<CourseResponseDto>
                {
                    new CourseResponseDto { Id = Guid.NewGuid(), Title = "Course 1", Cost = 100, Description = "Description 1" , Status = CourseStatus.Done, PictureUrl = "http://example.com/course1.jpg"},
                    new CourseResponseDto { Id = Guid.NewGuid(), Title = "Course 2", Cost = 100, Description = "Description 1" , Status = CourseStatus.Done, PictureUrl = "http://example.com/course1.jpg"}
                },
                TotalCount = 2,
                PageSize = 10,
                PageNumber = 1
            };

            _courseRepositoryMock
                .Setup(repo => repo.GetAllAsync(queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var query = new GetCoursesQuery(queryParams);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(expected.TotalCount);
            result.Items.Should().HaveCount(expected.Items.Count);
            result.Items[0].Title.Should().Be("Course 1");
        }
    }
}