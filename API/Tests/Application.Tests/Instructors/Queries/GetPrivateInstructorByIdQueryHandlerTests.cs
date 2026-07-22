using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;
using Application.Features.Instructors.Queries.GetPrivateById;
using Domain.Entities.Identity;
using Domain.Enums.Identity;
using FluentAssertions;
using Moq;

namespace Application.Tests.Instructors.Queries;

public class GetPrivateInstructorByIdQueryHandlerTests
{
    private readonly Mock<IInstructorRepository> _instructorRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUserIdentityService> _userIdentityServiceMock;
    private readonly GetPrivateInstructorByIdQueryHandler _handler;

    public GetPrivateInstructorByIdQueryHandlerTests()
    {
        _instructorRepositoryMock = new Mock<IInstructorRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _userIdentityServiceMock = new Mock<IUserIdentityService>();

        _currentUserServiceMock.Setup(x => x.UserId).Returns("admin-id");
        var adminUser = new ApplicationUser { Id = "admin-id" };
        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync("admin-id")).ReturnsAsync(adminUser);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(adminUser, Role.Admin.ToString())).ReturnsAsync(true);

        _handler = new GetPrivateInstructorByIdQueryHandler(
            _instructorRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _userIdentityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInstructorDto_WhenUserIsAdmin()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var userId = "admin123";
        var query = new GetPrivateInstructorByIdQuery(instructorId);
        var expectedDto = new InstructorPrivateResponseDto 
        { 
            Id = instructorId,
            Bio = "Test Bio",
            Title = "Test Title"
        };
        var user = new ApplicationUser { Id = userId };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId)).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);
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
    public async Task Handle_ShouldReturnInstructorDto_WhenUserIsSuperAdmin()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var userId = "superadmin123";
        var query = new GetPrivateInstructorByIdQuery(instructorId);
        var expectedDto = new InstructorPrivateResponseDto 
        { 
            Id = instructorId,
            Bio = "Test Bio",
            Title = "Test Title"
        };
        var user = new ApplicationUser { Id = userId };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId)).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(user, "SuperAdmin")).ReturnsAsync(true);
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
    public async Task Handle_ShouldReturnInstructorDto_WhenUserIsInstructorOwner()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var userId = "instructor123";
        var query = new GetPrivateInstructorByIdQuery(instructorId);
        var expectedDto = new InstructorPrivateResponseDto 
        { 
            Id = instructorId,
            Bio = "Test Bio",
            Title = "Test Title"
        };
        var user = new ApplicationUser { Id = userId };
        var instructor = new Domain.Entities.Identity.Instructor { Id = instructorId, UserId = userId };

        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId)).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(user, "SuperAdmin")).ReturnsAsync(false);
        _instructorRepositoryMock.Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(instructor);
        _instructorRepositoryMock.Setup(x => x.GetPrivateByIdAsync(instructorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        _instructorRepositoryMock.Verify(x => x.GetPrivateByIdAsync(instructorId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
