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

        _cacheMock
            .Setup(cache => cache.GetOrCreateAsync<InstructorPrivateResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, ValueTask<InstructorPrivateResponseDto?>>>(),
                It.IsAny<CacheOptions>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns((string key, Func<CancellationToken, ValueTask<InstructorPrivateResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
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
        var query = new GetPrivateInstructorByIdQuery(instructorId);

        _instructorRepositoryMock.Setup(x => x.GetPrivateByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InstructorPrivateResponseDto?)null);

        _cacheMock
            .Setup(cache => cache.GetOrCreateAsync<InstructorPrivateResponseDto>(
                It.IsAny<string>(),
                It.IsAny<Func<CancellationToken, ValueTask<InstructorPrivateResponseDto?>>>(),
                It.IsAny<CacheOptions>(),
                It.IsAny<string[]>(),
                It.IsAny<CancellationToken>()))
            .Returns((string key, Func<CancellationToken, ValueTask<InstructorPrivateResponseDto?>> factory, CacheOptions options, string[] tags, CancellationToken ct) => {
                var result = factory(ct);
                return factory(ct);
            });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
