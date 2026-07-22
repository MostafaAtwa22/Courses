using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;
using Application.Features.Instructors.Commands.Create;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.Tests.Instructors.Commands;

public class CreateInstructorCommandHandlerTests
{
    private readonly Mock<IInstructorRepository> _instructorRepositoryMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly CreateInstructorCommandHandler _handler;

    public CreateInstructorCommandHandlerTests()
    {
        _instructorRepositoryMock = new Mock<IInstructorRepository>();
        _fileServiceMock = new Mock<IFileService>();

        _handler = new CreateInstructorCommandHandler(
            _instructorRepositoryMock.Object,
            _fileServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInstructorId_WhenCreationIsSuccessful()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var userId = "user123";
        var cvUrl = "https://example.com/cv.pdf";
        var dto = new InstructorCreateDto 
        { 
            Bio = "Test Bio",
            Title = "Test Title",
            LinkedInProfileUrl = "https://linkedin.com/test",
            GitHubProfileUrl = "https://github.com/test",
            CvUrl = MockHelpers.CreateMockFormFile("cv.pdf")
        };
        var user = new ApplicationUser { Id = userId };
        var command = new CreateInstructorCommand(dto);
        command.User = user;

        _fileServiceMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(cvUrl);
        _instructorRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Instructor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(instructorId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(instructorId);
        _fileServiceMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _instructorRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Instructor>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUploadCv_WhenCvFileIsProvided()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var userId = "user123";
        var cvUrl = "https://example.com/cv.pdf";
        var dto = new InstructorCreateDto 
        { 
            Bio = "Test Bio",
            Title = "Test Title",
            LinkedInProfileUrl = "https://linkedin.com/test",
            GitHubProfileUrl = "https://github.com/test",
            CvUrl = MockHelpers.CreateMockFormFile("cv.pdf")
        };
        var user = new ApplicationUser { Id = userId };
        var command = new CreateInstructorCommand(dto);
        command.User = user;

        _fileServiceMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(cvUrl);
        _instructorRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Instructor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(instructorId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), "cv.pdf", It.IsAny<string>()), Times.Once);
    }
}

