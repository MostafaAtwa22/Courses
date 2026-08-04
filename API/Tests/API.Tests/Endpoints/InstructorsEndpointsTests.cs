using API.Endpoints;
using Application.Common.Models;
using Application.DTOs.Instructor;
using Application.Features.Instructors.Commands.ChangeStatus;
using Application.Features.Instructors.Commands.Create;
using Application.Features.Instructors.Commands.Delete;
using Application.Features.Instructors.Commands.Update;
using Application.Features.Instructors.Queries.GetAll;
using Application.Features.Instructors.Queries.GetPrivateById;
using Application.Features.Instructors.Queries.GetPublicById;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class InstructorsEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public InstructorsEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetPublicInstructor_ShouldReturnOk_WhenInstructorExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedInstructor = new InstructorPublicResponseDto { Id = id, Title = "Test Instructor" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPublicInstructorByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInstructor);

            // Act
            var result = await InstructorsEndpoints.GetPublicInstructor(id, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<InstructorPublicResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedInstructor);
        }

        [Fact]
        public async Task GetPublicInstructor_ShouldReturnNotFound_WhenInstructorDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPublicInstructorByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InstructorPublicResponseDto)null!);

            // Act
            var result = await InstructorsEndpoints.GetPublicInstructor(id, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetPrivateInstructor_ShouldReturnOk_WhenInstructorExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedInstructor = new InstructorPrivateResponseDto { Id = id, Title = "Test Instructor" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPrivateInstructorByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInstructor);

            // Act
            var result = await InstructorsEndpoints.GetPrivateInstructor(id, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<InstructorPrivateResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedInstructor);
        }

        [Fact]
        public async Task GetPrivateInstructor_ShouldReturnNotFound_WhenInstructorDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPrivateInstructorByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((InstructorPrivateResponseDto)null!);

            // Act
            var result = await InstructorsEndpoints.GetPrivateInstructor(id, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateInstructor_ShouldReturnCreatedAtRoute_WhenSuccessful()
        {
            // Arrange
            var request = new InstructorCreateDto { Title = "New Instructor" };
            var newId = Guid.NewGuid();
            var expectedInstructor = new InstructorPrivateResponseDto { Id = newId, Title = "New Instructor" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateInstructorCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPrivateInstructorByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedInstructor);

            // Act
            var result = await InstructorsEndpoints.CreateInstructor(request, _mediatorMock.Object);

            // Assert
            var createdResult = result.Result as CreatedAtRoute<InstructorPrivateResponseDto>;
            createdResult.Should().NotBeNull();
            createdResult!.RouteName.Should().Be(nameof(InstructorsEndpoints.GetPrivateInstructor));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(newId);
            createdResult.Value.Should().Be(expectedInstructor);
        }

        [Fact]
        public async Task UpdateInstructor_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new InstructorUpdateDto { Title = "Updated Instructor" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateInstructorCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await InstructorsEndpoints.UpdateInstructor(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAllInstructors_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var queryParams = new InstructorQueryParams();
            var expectedResult = new PaginatedResult<InstructorPrivateResponseDto>(new List<InstructorPrivateResponseDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllInstructorsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await InstructorsEndpoints.GetAllInstructors(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result as Ok<PaginatedResult<InstructorPrivateResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task ChangeInstructorStatus_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new ChangeInstructorStatusDto { Status = Domain.Enums.InstructorStatus.Verfied };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangeInstructorStatusCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await InstructorsEndpoints.ChangeInstructorStatus(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteInstructor_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteInstructorCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await InstructorsEndpoints.DeleteInstructor(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
