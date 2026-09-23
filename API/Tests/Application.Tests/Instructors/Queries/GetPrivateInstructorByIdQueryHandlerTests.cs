using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;
using Application.Features.Instructors.Queries.GetPrivateById;
using FluentAssertions;
using Moq;

namespace Application.Tests.Instructors.Queries;

public class GetPrivateInstructorByIdQueryHandlerTests
{
    private readonly Mock<IInstructorRepository> _instructorRepositoryMock;
    private readonly Mock<IAppCache> _cacheMock;
    private readonly GetPrivateInstructorByIdQueryHandler _handler;

    public GetPrivateInstructorByIdQueryHandlerTests()
    {
        _instructorRepositoryMock = new Mock<IInstructorRepository>();
        _cacheMock = new Mock<IAppCache>();

        _handler = new GetPrivateInstructorByIdQueryHandler(
            _instructorRepositoryMock.Object,
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInstructorDto_WhenInstructorExists()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var query = new GetPrivateInstructorByIdQuery(instructorId);
        var expectedDto = new InstructorPrivateResponseDto
        {
            Id = instructorId,
            Bio = "Test Bio",
            Title = "Test Title"
        };

        _instructorRepositoryMock.Setup(x => x.GetPrivateByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        _instructorRepositoryMock.Verify(x => x.GetPrivateByIdAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenInstructorDoesNotExist()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var query = new GetPrivateInstructorByIdQuery(instructorId);

        _instructorRepositoryMock.Setup(x => x.GetPrivateByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InstructorPrivateResponseDto?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _instructorRepositoryMock.Verify(x => x.GetPrivateByIdAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
