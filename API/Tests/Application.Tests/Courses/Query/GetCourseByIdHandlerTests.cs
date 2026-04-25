using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.DTOs.Course;
using Application.Features.Courses.Queries.GetById;
using Domain.Entities;
using FluentAssertions;
using Moq;

namespace Application.Tests.Courses.Query
{
    public class GetCourseByIdHandlerTests
    {
        private readonly Mock<ICourseRepository> _repoMock;
        private readonly GetCourseByIdQueryHandler _handler;

        public GetCourseByIdHandlerTests()
        {
            _repoMock = new Mock<ICourseRepository>();
            _handler = new GetCourseByIdQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnsCourse_WhenCourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var expectedCourse = new CoursesResponseDto
            {
                Id = courseId,
                Title = "Test Course",
                Description = "Test Description",
                Cost = 100,
                Status = Domain.Enums.CourseStatus.Done,
                PictureUrl = "http://example.com/course.jpg"
            };

            _repoMock.Setup(repo => repo.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expectedCourse);

            var query = new GetCourseByIdQuery(courseId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedCourse.Id);
            result.Title.Should().Be(expectedCourse.Title);
            result.Description.Should().Be(expectedCourse.Description);
            result.Cost.Should().Be(expectedCourse.Cost);
            result.Status.Should().Be(expectedCourse.Status);
            result.PictureUrl.Should().Be(expectedCourse.PictureUrl);
        }
        
        [Fact]
        public async Task Handle_ShouldReturnsNull_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _repoMock.Setup(repo => repo.GetByIdAsync(courseId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((CoursesResponseDto?)null);

            var query = new GetCourseByIdQuery(courseId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}