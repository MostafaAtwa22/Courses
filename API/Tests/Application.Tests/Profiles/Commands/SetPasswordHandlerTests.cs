using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.SetPassword;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;namespace Application.Tests.Profiles.Commands;

public class SetPasswordHandlerTests
{
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IUserIdentityService> _userIdentityServiceMock;
    private readonly SetPasswordCommandHandler _handler;

    public SetPasswordHandlerTests()
    {
        _passwordServiceMock     = new Mock<IPasswordService>();
        _userIdentityServiceMock = new Mock<IUserIdentityService>();
        _handler = new SetPasswordCommandHandler(_passwordServiceMock.Object, _userIdentityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSetPassword_WhenUserHasNoPassword()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new SetPasswordDto
        {
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new SetPasswordCommand(dto) { User = user };

        _passwordServiceMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(false);
        _passwordServiceMock.Setup(x => x.SetPasswordAsync(user, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordServiceMock.Verify(x => x.SetPasswordAsync(user, dto.NewPassword), Times.Once);
        _userIdentityServiceMock.Verify(x => x.UpdateSecurityStampAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenUserAlreadyHasPassword()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new SetPasswordDto
        {
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new SetPasswordCommand(dto) { User = user };

        _passwordServiceMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User already has a password");
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenIdentityFails()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new SetPasswordDto
        {
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new SetPasswordCommand(dto) { User = user };

        _passwordServiceMock.Setup(x => x.HasPasswordAsync(user)).ReturnsAsync(false);
        var identityError = new IdentityError { Description = "Password too short" };
        _passwordServiceMock.Setup(x => x.SetPasswordAsync(user, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Password too short");
    }
}
