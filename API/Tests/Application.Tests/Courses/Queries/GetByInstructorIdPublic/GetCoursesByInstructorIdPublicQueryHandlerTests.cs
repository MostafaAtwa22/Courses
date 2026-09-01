using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTOs.Course;
using Application.Features.Courses.Queries.GetByInstructorIdPublic;
using FluentAssertions;
using Moq;

namespace Application.Tests.Courses.Queries.GetByInstructorIdPublic
{
    public class GetCoursesByInstructorIdPublicQueryHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly GetCoursesByInstructorIdPublicQueryHandler _handler;

        public GetCoursesByInstructorIdPublicQueryHandlerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _handler = new GetCoursesByInstructorIdPublicQueryHandler(_courseRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnPublishedCourses_WhenInstructorExists()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var queryParams = new CourseQueryParams();
            var expectedCourses = new List<CourseSummaryDto>
            {
                new() { Id = Guid.NewGuid(), Title = "Course 1" },
                new() { Id = Guid.NewGuid(), Title = "Course 2" }
            };
            var expectedResult = new PaginatedResult<CourseSummaryDto>(expectedCourses, 2, 1, 10);
            var query = new GetCoursesByInstructorIdPublicQuery(instructorId, queryParams);

            _courseRepositoryMock.Setup(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            result.TotalCount.Should().Be(2);
            _courseRepositoryMock.Verify(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyResult_WhenNoPublishedCourses()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            var query = new GetCoursesByInstructorIdPublicQuery(instructorId, queryParams);

            _courseRepositoryMock.Setup(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(0);
            result.Items.Should().BeEmpty();
            _courseRepositoryMock.Verify(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldUseCorrectInstructorId()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            var query = new GetCoursesByInstructorIdPublicQuery(instructorId, queryParams);

            _courseRepositoryMock.Setup(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _courseRepositoryMock.Verify(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldPassQueryParamsCorrectly()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var queryParams = new CourseQueryParams { Category = "Programming", MinRating = 4.0m };
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            var query = new GetCoursesByInstructorIdPublicQuery(instructorId, queryParams);

            _courseRepositoryMock.Setup(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await _handler.Handle(query, CancellationToken.None);

            // Assert
            _courseRepositoryMock.Verify(x => x.GetPublishedCoursesByInstructorIdAsync(instructorId, queryParams, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
