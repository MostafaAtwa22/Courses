using Application.Common.Models;
using Application.DTOs.Section;
using Application.Features.Sections.Commands.Create;
using Application.Features.Sections.Commands.Delete;
using Application.Features.Sections.Commands.Update;
using Application.Features.Sections.Queries.GetAll;
using Application.Features.Sections.Queries.GetById;
using Application.Features.Sections.Queries.GetByCourseId;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using API.Endpoints;

namespace API.Tests.Endpoints
{
    public class SectionsEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public SectionsEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetSections_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var queryParams = new QueryParams();
            var expectedResult = new PaginatedResult<SectionResponseDto>(new List<SectionResponseDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSectionsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await SectionsEndpoints.GetSections(queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<SectionResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetSectionsByCourseId_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams();
            var expectedResult = new PaginatedResult<SectionResponseDto>(new List<SectionResponseDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSectionsByCourseIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await SectionsEndpoints.GetSectionsByCourseId(courseId, queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<SectionResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetSectionById_ShouldReturnOk_WhenSectionExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedSection = new SectionResponseDto { Id = id, Title = "Test Section" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSectionByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSection);

            // Act
            var result = await SectionsEndpoints.GetSectionById(id, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<SectionResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedSection);
        }

        [Fact]
        public async Task GetSectionById_ShouldReturnNotFound_WhenSectionDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSectionByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((SectionResponseDto)null!);

            // Act
            var result = await SectionsEndpoints.GetSectionById(id, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateSection_ShouldReturnCreatedAtRoute_WhenSuccessful()
        {
            // Arrange
            var request = new SectionCreateDto { Title = "New Section" };
            var newId = Guid.NewGuid();
            var expectedSection = new SectionResponseDto { Id = newId, Title = "New Section" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateSectionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetSectionByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedSection);

            // Act
            var result = await SectionsEndpoints.CreateSection(request, _mediatorMock.Object);

            // Assert
            var createdResult = result.Result as CreatedAtRoute<SectionResponseDto>;
            createdResult.Should().NotBeNull();
            createdResult!.RouteName.Should().Be(nameof(SectionsEndpoints.GetSectionById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(newId);
            createdResult.Value.Should().Be(expectedSection);
        }

        [Fact]
        public async Task UpdateSection_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new SectionUpdateDto { Title = "Updated Section" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateSectionCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await SectionsEndpoints.UpdateSection(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteSection_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteSectionCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await SectionsEndpoints.DeleteSection(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
