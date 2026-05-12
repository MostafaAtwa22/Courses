using Application.Common.Exceptions;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.ChangePassword;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Profiles.Commands;

public class ChangePasswordHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly ChangePasswordCommandHandler _handler;

    public ChangePasswordHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _handler = new ChangePasswordCommandHandler(_userManagerMock.Object);
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

        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword), Times.Once);
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
        _userManagerMock.Setup(x => x.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(identityError));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Invalid password");
    }
}
