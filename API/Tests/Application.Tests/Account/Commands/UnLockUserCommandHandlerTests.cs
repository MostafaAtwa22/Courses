using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Features.Account.Commands.UnLock;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;namespace Application.Tests.Account.Commands;

public class UnLockUserCommandHandlerTests
{
    private readonly Mock<IUserIdentityService> _userIdentityServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IIdentityEmailService> _identityEmailServiceMock;
    private readonly UnLockUserCommandHandler _handler;

    public UnLockUserCommandHandlerTests()
    {
        _userIdentityServiceMock = new Mock<IUserIdentityService>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _identityEmailServiceMock = new Mock<IIdentityEmailService>();
        _handler = new UnLockUserCommandHandler(_userIdentityServiceMock.Object, _passwordServiceMock.Object, _identityEmailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUnlockUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UnLockUserCommand(userId);
        var user = new ApplicationUser { Id = userId.ToString() };

        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId.ToString())).ReturnsAsync(user);
        _passwordServiceMock.Setup(x => x.UnLockUserAsync(user)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordServiceMock.Verify(x => x.UnLockUserAsync(user), Times.Once);
        _userIdentityServiceMock.Verify(x => x.ResetAccessFailedCountAsync(user), Times.Once);
        _identityEmailServiceMock.Verify(x => x.SendAccountUnlockedEmailAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new UnLockUserCommand(userId);
        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
