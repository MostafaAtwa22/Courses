using Application.Common.Exceptions;
using Application.DTOs.Profile;
using Application.Features.Profiles.Commands.Delete;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Profiles.Commands;

public class DeleteProfileHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly DeleteProfileHandler _handler;

    public DeleteProfileHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _handler = new DeleteProfileHandler(_userManagerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteProfile_WhenPasswordIsCorrect()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new DeleteProfileDto { Password = "ValidPassword123!" };
        var command = new DeleteProfileCommand(dto) { User = user };

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userManagerMock.Verify(x => x.UpdateSecurityStampAsync(user), Times.Once);
        _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenPasswordIsIncorrect()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-id" };
        var dto = new DeleteProfileDto { Password = "WrongPassword" };
        var command = new DeleteProfileCommand(dto) { User = user };

        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Invalid password");
    }
}
