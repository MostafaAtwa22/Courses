using Application.Common.Interfaces;
using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;
using Application.Features.Instructors.Queries.GetPublicById;
using FluentAssertions;
using Moq;

namespace Application.Tests.Instructors.Queries;

public class GetPublicInstructorByIdQueryHandlerTests
{
    private readonly Mock<IInstructorRepository> _instructorRepositoryMock;
    private readonly Mock<IAppCache> _cacheMock;
    private readonly GetPublicInstructorByIdQueryHandler _handler;

    public GetPublicInstructorByIdQueryHandlerTests()
    {
        _instructorRepositoryMock = new Mock<IInstructorRepository>();
        _cacheMock = new Mock<IAppCache>();
        _handler = new GetPublicInstructorByIdQueryHandler(_instructorRepositoryMock.Object, _cacheMock.Object);
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

        _cacheMock
            .Setup(cache => cache.GetOrCreateAsync<InstructorPublicResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, ValueTask<InstructorPublicResponseDto?>>>(),
                It.IsAny<CacheOptions>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns((string key, Func<CancellationToken, ValueTask<InstructorPublicResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                var result = factory(ct);
                return factory(ct);
            });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenInstructorDoesNotExist()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var query = new GetPublicInstructorByIdQuery(instructorId);

        _instructorRepositoryMock.Setup(x => x.GetPublicByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InstructorPublicResponseDto?)null);

        _cacheMock
            .Setup(cache => cache.GetOrCreateAsync<InstructorPublicResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, ValueTask<InstructorPublicResponseDto?>>>(),
                It.IsAny<CacheOptions>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns((string key, Func<CancellationToken, ValueTask<InstructorPublicResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                var result = factory(ct);
                return factory(ct);
            });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
