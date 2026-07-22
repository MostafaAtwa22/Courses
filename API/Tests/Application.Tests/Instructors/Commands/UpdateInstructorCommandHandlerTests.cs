using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;
using Application.Features.Instructors.Commands.Update;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.Tests.Instructors.Commands;

public class UpdateInstructorCommandHandlerTests
{
    private readonly Mock<IInstructorRepository> _instructorRepositoryMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly UpdateInstructorCommandHandler _handler;

    public UpdateInstructorCommandHandlerTests()
    {
        _instructorRepositoryMock = new Mock<IInstructorRepository>();
        _fileServiceMock = new Mock<IFileService>();

        _handler = new UpdateInstructorCommandHandler(
            _instructorRepositoryMock.Object,
            _fileServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUpdateInstructor_WhenInstructorExists()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var oldCvUrl = "https://example.com/old_cv.pdf";
        var newCvUrl = "https://example.com/new_cv.pdf";
        var dto = new InstructorUpdateDto 
        { 
            Bio = "Updated Bio",
            Title = "Updated Title",
            LinkedInProfileUrl = "https://linkedin.com/updated",
            GitHubProfileUrl = "https://github.com/updated",
            CvUrl = MockHelpers.CreateMockFormFile("new_cv.pdf")
        };
        var command = new UpdateInstructorCommand(instructorId, dto);
        var instructor = new Instructor 
        { 
            Id = instructorId,
            CvUrl = oldCvUrl
        };

        _instructorRepositoryMock.Setup(x => x.GetEntityByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(instructor);
        _fileServiceMock.Setup(x => x.DeleteAsync(oldCvUrl))
            .Returns(Task.CompletedTask);
        _fileServiceMock.Setup(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(newCvUrl);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(x => x.DeleteAsync(oldCvUrl), Times.Once);
        _fileServiceMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), "new_cv.pdf", It.IsAny<string>()), Times.Once);
        _instructorRepositoryMock.Verify(x => x.UpdateAsync(instructor, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateInstructorWithoutCv_WhenCvNotProvided()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var dto = new InstructorUpdateDto 
        { 
            Bio = "Updated Bio",
            Title = "Updated Title",
            LinkedInProfileUrl = "https://linkedin.com/updated",
            GitHubProfileUrl = "https://github.com/updated",
            CvUrl = null
        };
        var command = new UpdateInstructorCommand(instructorId, dto);
        var instructor = new Instructor 
        { 
            Id = instructorId,
            CvUrl = "https://example.com/old_cv.pdf"
        };

        _instructorRepositoryMock.Setup(x => x.GetEntityByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(instructor);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Never);
        _fileServiceMock.Verify(x => x.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _instructorRepositoryMock.Verify(x => x.UpdateAsync(instructor, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenInstructorDoesNotExist()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var dto = new InstructorUpdateDto 
        { 
            Bio = "Updated Bio",
            Title = "Updated Title",
            LinkedInProfileUrl = "https://linkedin.com/updated",
            GitHubProfileUrl = "https://github.com/updated",
            CvUrl = null
        };
        var command = new UpdateInstructorCommand(instructorId, dto);

        _instructorRepositoryMock.Setup(x => x.GetEntityByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Instructor?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Entity \"Instructor\" with key \"{instructorId}\" was not found.");
    }
}

