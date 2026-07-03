using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.ChangePassword;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;namespace Application.Tests.Profiles.Commands;

public class ChangePasswordHandlerTests
{
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IUserIdentityService> _userIdentityServiceMock;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordHandlerTests()
    {
        _passwordServiceMock     = new Mock<IPasswordService>();
        _userIdentityServiceMock = new Mock<IUserIdentityService>();
        _handler = new ChangePasswordCommandHandler(_passwordServiceMock.Object, _userIdentityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldChangePassword_WhenDataIsValid()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new ChangePasswordDto
        {
            OldPassword = "OldPassword123!",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new ChangePasswordCommand(dto) { User = user };

        _passwordServiceMock.Setup(x => x.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordServiceMock.Verify(x => x.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword), Times.Once);
        _userIdentityServiceMock.Verify(x => x.UpdateSecurityStampAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenIdentityFails()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new ChangePasswordDto
        {
            OldPassword = "WrongPassword",
            NewPassword = "NewPassword123!",
            ConfirmNewPassword = "NewPassword123!"
        };
        var command = new ChangePasswordCommand(dto) { User = user };

        var identityError = new IdentityError { Description = "Invalid password" };
        _passwordServiceMock.Setup(x => x.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Invalid password");
    }
}
