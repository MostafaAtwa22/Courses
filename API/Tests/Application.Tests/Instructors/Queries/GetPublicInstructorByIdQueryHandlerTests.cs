using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;
using Application.Features.Instructors.Queries.GetPublicById;
using FluentAssertions;
using Moq;

namespace Application.Tests.Instructors.Queries;

public class GetPublicInstructorByIdQueryHandlerTests
{
    private readonly Mock<IInstructorRepository> _instructorRepositoryMock;
    private readonly GetPublicInstructorByIdQueryHandler _handler;

    public GetPublicInstructorByIdQueryHandlerTests()
    {
        _instructorRepositoryMock = new Mock<IInstructorRepository>();
        _handler = new GetPublicInstructorByIdQueryHandler(_instructorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInstructorDto_WhenInstructorExists()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var query = new GetPublicInstructorByIdQuery(instructorId);
        var expectedDto = new InstructorPublicResponseDto 
        { 
            Id = instructorId,
            Bio = "Test Bio",
            Title = "Test Title"
        };

        _instructorRepositoryMock.Setup(x => x.GetPublicByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        _instructorRepositoryMock.Verify(x => x.GetPublicByIdAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenInstructorDoesNotExist()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var query = new GetPublicInstructorByIdQuery(instructorId);

        _instructorRepositoryMock.Setup(x => x.GetPublicByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InstructorPublicResponseDto?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _instructorRepositoryMock.Verify(x => x.GetPublicByIdAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
