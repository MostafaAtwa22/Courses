using API.Endpoints;
using Application.Common.Models;
using Application.DTOs.Student;
using Application.Features.Student.Commands.DeleteStudent;
using Application.Features.Student.Queries.GetAll;
using Application.Features.Student.Queries.GetById;
using Application.Features.Student.Queries.GetByUserId;
using Domain.Enums.Identity;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class StudentEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public StudentEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturnOk_WhenStudentsExist()
        {
            // Arrange
            var students = new List<StudentResponseDto>
            {
                new StudentResponseDto { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
                new StudentResponseDto { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
            };

            var paginatedResult = new PaginatedResult<StudentResponseDto>(students, 2, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResult);

            // Act
            var result = await StudentEndpoints.GetAllStudents(It.IsAny<StudentQueryParams>(), _mediatorMock.Object);

            // Assert
            var okResult = result.Should().BeAssignableTo<Results<Ok<PaginatedResult<StudentResponseDto>>, BadRequest>>().Subject;
            okResult.Result.Should().BeOfType<Ok<PaginatedResult<StudentResponseDto>>>();
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnOk_WhenStudentExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var student = new StudentResponseDto { Id = studentId, FirstName = "John", LastName = "Doe" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(student);

            // Act
            var result = await StudentEndpoints.GetStudentById(studentId, _mediatorMock.Object);

            // Assert
            var okResult = result.Should().BeAssignableTo<Results<Ok<StudentResponseDto>, NotFound>>().Subject;
            okResult.Result.Should().BeOfType<Ok<StudentResponseDto>>();
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnNotFound_WhenStudentNotExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((StudentResponseDto?)null);

            // Act
            var result = await StudentEndpoints.GetStudentById(studentId, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Should().BeAssignableTo<Results<Ok<StudentResponseDto>, NotFound>>().Subject;
            notFoundResult.Result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task GetStudentByUserId_ShouldReturnOk_WhenStudentExists()
        {
            // Arrange
            var userId = "user-123";
            var student = new StudentResponseDto { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentByUserIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(student);

            // Act
            var result = await StudentEndpoints.GetStudentByUserId(userId, _mediatorMock.Object);

            // Assert
            var okResult = result.Should().BeAssignableTo<Results<Ok<StudentResponseDto>, NotFound>>().Subject;
            okResult.Result.Should().BeOfType<Ok<StudentResponseDto>>();
        }

        [Fact]
        public async Task GetStudentByUserId_ShouldReturnNotFound_WhenStudentNotExists()
        {
            // Arrange
            var userId = "non-existent-user";

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetStudentByUserIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((StudentResponseDto?)null);

            // Act
            var result = await StudentEndpoints.GetStudentByUserId(userId, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Should().BeAssignableTo<Results<Ok<StudentResponseDto>, NotFound>>().Subject;
            notFoundResult.Result.Should().BeOfType<NotFound>();
        }

        [Fact]
        public async Task DeleteStudent_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteStudentCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await StudentEndpoints.DeleteStudent(studentId, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Should().BeAssignableTo<Results<NoContent, NotFound>>().Subject;
            noContentResult.Result.Should().BeOfType<NoContent>();
        }
    }
}
