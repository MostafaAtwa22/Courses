using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.Register;
using Domain.Entities.Identity;
using Domain.Enums;
using Domain.Enums.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Authentication.Commands;

public class RegisterHandlerTests
{
    private readonly Mock<IUserCreationService> _userCreationServiceMock;
    private readonly CreateRegisterCommandHandler _handler;

    public RegisterHandlerTests()
    {
        _userCreationServiceMock = new Mock<IUserCreationService>();

        _handler = new CreateRegisterCommandHandler(
            _userCreationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenDataIsValid()
    {
        // Arrange
        var dto = new RegisterDto
        {
            Email    = "test@example.com",
            UserName = "testuser",
            Password = "Password123!",
            FirstName = "Test",
            LastName  = "User",
            Role      = Role.Student
        };
        var command = new CreateRegisterCommand(dto);

        _userCreationServiceMock.Setup(x => x.CreateUserAsync(
            It.IsAny<RegisterDto>(),
            It.IsAny<UserCreationOptions>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ApplicationUser());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userCreationServiceMock.Verify(x => x.CreateUserAsync(
            It.Is<RegisterDto>(d => d.Email == dto.Email),
            It.Is<UserCreationOptions>(o => 
                o.ConfirmEmail == true && 
                o.CreateStudentProfile == true && 
                o.AutoConfirmEmail == false),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenEmailExists()
    {
        // Arrange
        var dto = new RegisterDto { Email = "exists@example.com" };
        var command = new CreateRegisterCommand(dto);
        _userCreationServiceMock.Setup(x => x.CreateUserAsync(
            It.IsAny<RegisterDto>(),
            It.IsAny<UserCreationOptions>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadRequestException("Email already exists."));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Email already exists.");
    }
}
