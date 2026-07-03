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
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IUserIdentityService> _userIdentityServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IIdentityEmailService> _identityEmailServiceMock;
    private readonly CreateRegisterCommandHandler _handler;

    public RegisterHandlerTests()
    {
        _userManagerMock          = MockHelpers.MockUserManager<ApplicationUser>();
        _userIdentityServiceMock  = new Mock<IUserIdentityService>();
        _passwordServiceMock      = new Mock<IPasswordService>();
        _identityEmailServiceMock = new Mock<IIdentityEmailService>();

        _handler = new CreateRegisterCommandHandler(
            _userManagerMock.Object,
            _userIdentityServiceMock.Object,
            _passwordServiceMock.Object,
            _identityEmailServiceMock.Object);
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

        _userIdentityServiceMock.Setup(x => x.IsEmailExistsAsync(dto.Email)).ReturnsAsync(false);
        _userIdentityServiceMock.Setup(x => x.IsUserNameExistsAsync(dto.UserName)).ReturnsAsync(false);
        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Student"), Times.Once);
        _passwordServiceMock.Verify(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()), Times.Once);
        _identityEmailServiceMock.Verify(x => x.SendEmailConfirmationEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenEmailExists()
    {
        // Arrange
        var dto = new RegisterDto { Email = "exists@example.com" };
        var command = new CreateRegisterCommand(dto);
        _userIdentityServiceMock.Setup(x => x.IsEmailExistsAsync(dto.Email)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Email already exists.");
    }
}
