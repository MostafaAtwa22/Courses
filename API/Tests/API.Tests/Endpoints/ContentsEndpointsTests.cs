using API.Endpoints;
using Application.Common.Models;
using Application.DTOs.Content;
using Application.Features.Contents.Commands.Create;
using Application.Features.Contents.Commands.Delete;
using Application.Features.Contents.Commands.Update;
using Application.Features.Contents.Queries.GetByCourse;
using Application.Features.Contents.Queries.GetById;
using Application.Features.Contents.Queries.GetBySection;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace API.Tests.Endpoints
{
    public class ContentsEndpointsTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public ContentsEndpointsTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task GetBySection_ShouldReturnOk_WithContentsList()
        {
            // Arrange
            var sectionId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedContents = new List<ContentResponseDto>
            {
                new ContentResponseDto { Id = Guid.NewGuid(), Title = "Content 1", SectionId = sectionId },
                new ContentResponseDto { Id = Guid.NewGuid(), Title = "Content 2", SectionId = sectionId }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetContentBySectionQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedContents);

            // Act
            var result = await ContentsEndpoints.GetBySection(sectionId, courseId, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<IReadOnlyList<ContentResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedContents);
        }

        [Fact]
        public async Task GetByCourse_ShouldReturnOk_WithPaginatedResult()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var queryParams = new QueryParams();
            var expectedResult = new PaginatedResult<ContentResponseDto>(new List<ContentResponseDto>(), 0, 1, 10);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetContentByCourseQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await ContentsEndpoints.GetByCourse(courseId, queryParams, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<PaginatedResult<ContentResponseDto>>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetContentById_ShouldReturnOk_WhenContentExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var expectedContent = new ContentResponseDto { Id = id, Title = "Test Content" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetContentByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedContent);

            // Act
            var result = await ContentsEndpoints.GetContentById(id, courseId, _mediatorMock.Object);

            // Assert
            var okResult = result.Result as Ok<ContentResponseDto>;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(expectedContent);
        }

        [Fact]
        public async Task GetContentById_ShouldReturnNotFound_WhenContentDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetContentByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ContentResponseDto)null!);

            // Act
            var result = await ContentsEndpoints.GetContentById(id, courseId, _mediatorMock.Object);

            // Assert
            var notFoundResult = result.Result as NotFound;
            notFoundResult.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateContent_ShouldReturnCreatedAtRoute_WhenSuccessful()
        {
            // Arrange
            var request = new ContentCreateDto { Title = "New Content" };
            var newId = Guid.NewGuid();
            var expectedContent = new ContentResponseDto { Id = newId, Title = "New Content" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateContentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newId);
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetContentByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedContent);

            // Act
            var result = await ContentsEndpoints.CreateContent(request, _mediatorMock.Object);

            // Assert
            var createdResult = result.Result as CreatedAtRoute<ContentResponseDto>;
            createdResult.Should().NotBeNull();
            createdResult!.RouteName.Should().Be(nameof(ContentsEndpoints.GetContentById));
            createdResult.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(newId);
            createdResult.Value.Should().Be(expectedContent);
        }

        [Fact]
        public async Task UpdateContent_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new ContentUpdateDto { Title = "Updated Content" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateContentCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ContentsEndpoints.UpdateContent(id, request, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteContent_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteContentCommand>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await ContentsEndpoints.DeleteContent(id, _mediatorMock.Object);

            // Assert
            var noContentResult = result.Result as NoContent;
            noContentResult.Should().NotBeNull();
        }
    }
}
