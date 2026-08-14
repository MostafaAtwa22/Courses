using Application.Common.Models;
using Application.DTOs.Course;
using Application.Features.Courses.Commands.Create;
using Application.Features.Courses.Commands.Delete;
using Application.Features.Courses.Commands.Update;
using Application.Features.Courses.Queries.GetAll;
using Application.Features.Courses.Queries.GetById;
using Application.Features.Courses.Queries.GetSuggestions;
using Application.Features.Courses.Queries.GetByStudentId;
using Application.Features.Courses.Queries.GetByInstructorId;
using Application.Features.Courses.Queries.GetByInstructorIdPublic;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using API.Endpoints;
using System.Collections.Generic;

namespace API.Tests.Endpoints
{
    public class CoursesEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public CoursesEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetCourses_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CoursesEndpoints.GetCourses(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CourseSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetSuggestions_ShouldReturnOk_WithSuggestions()
        {
            // Arrange
            var query = "test";
            var expectedSuggestions = new List<string> { "Test Course 1", "Test Course 2" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseSuggestionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSuggestions);

            // Act
            var result = await CoursesEndpoints.GetSuggestions(query, _mediatorMock.Object);

            // Assert
            result.Should().BeOfType<Ok<IEnumerable<string>>>();
            result.Value.Should().BeEquivalentTo(expectedSuggestions);
        }

        [Fact]
        public async Task GetCourseById_ShouldReturnOk_WhenCourseExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedCourse = new CourseResponseDto();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await CoursesEndpoints.GetCourseById(id, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<CourseResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedCourse);
        }

        [Fact]
        public async Task GetCourseById_ShouldReturnNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((CourseResponseDto)null!);

            // Act
            var result = await CoursesEndpoints.GetCourseById(id, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateCourse_ShouldReturnCreatedAtRoute_WhenSuccessful()
        {
            // Arrange
            var request = new CourseCreateDto();
            var newId = Guid.NewGuid();
            var expectedCourse = new CourseResponseDto { Id = newId };
            
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCourseByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await CoursesEndpoints.CreateCourse(request, _mediatorMock.Object);

            // Assert
            var createdResult = result.Result as CreatedAtRoute<CourseResponseDto>;
            createdResult.Should().NotBeNull();
            createdResult!.RouteName.Should().Be(nameof(CoursesEndpoints.GetCourseById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(newId);
            createdResult.Value.Should().Be(expectedCourse);
        }

        [Fact]
        public async Task UpdateCourse_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new CourseUpdateDto();
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await CoursesEndpoints.UpdateCourse(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteCourse_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteCourseCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await CoursesEndpoints.DeleteCourse(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCoursesByStudentId_ShouldReturnOk_WhenCoursesExist()
        {
            // Arrange
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 1, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesByStudentIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CoursesEndpoints.GetCoursesByStudentId(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CourseSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCoursesByStudentId_ShouldReturnOk_WhenNoCourses()
        {
            // Arrange
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesByStudentIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CoursesEndpoints.GetCoursesByStudentId(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CourseSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task GetCoursesByInstructorId_ShouldReturnOk_WhenCoursesExist()
        {
            // Arrange
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 1, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesByInstructorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CoursesEndpoints.GetCoursesByInstructorId(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CourseSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCoursesByInstructorId_ShouldReturnOk_WhenNoCourses()
        {
            // Arrange
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesByInstructorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CoursesEndpoints.GetCoursesByInstructorId(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CourseSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task GetCoursesByInstructorIdPublic_ShouldReturnOk_WhenCoursesExist()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 2, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesByInstructorIdPublicQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CoursesEndpoints.GetCoursesByInstructorIdPublic(instructorId, queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CourseSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetCoursesByInstructorIdPublic_ShouldReturnOk_WhenNoCourses()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesByInstructorIdPublicQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await CoursesEndpoints.GetCoursesByInstructorIdPublic(instructorId, queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<CourseSummaryDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.TotalCount.Should().Be(0);
        }

        [Fact]
        public async Task GetCoursesByInstructorIdPublic_ShouldUseCorrectInstructorId()
        {
            // Arrange
            var instructorId = Guid.NewGuid();
            var queryParams = new CourseQueryParams();
            var expectedResult = new PaginatedResult<CourseSummaryDto>(new List<CourseSummaryDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetCoursesByInstructorIdPublicQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            await CoursesEndpoints.GetCoursesByInstructorIdPublic(instructorId, queryParams, _mediatorMock.Object);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetCoursesByInstructorIdPublicQuery>(q => q.InstructorId == instructorId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
