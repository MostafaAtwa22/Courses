using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Account;
using Application.Features.Account.Commands.Lock;
using Domain.Entities.Identity;
using Domain.Enums.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;namespace Application.Tests.Account.Commands;

public class LockUserCommandHandlerTests
{
    private readonly Mock<IUserIdentityService> _userIdentityServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IIdentityEmailService> _identityEmailServiceMock;
    private readonly LockUserCommandHandler _handler;

    public LockUserCommandHandlerTests()
    {
        _userIdentityServiceMock = new Mock<IUserIdentityService>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _identityEmailServiceMock = new Mock<IIdentityEmailService>();
        _handler = new LockUserCommandHandler(_userIdentityServiceMock.Object, _passwordServiceMock.Object, _identityEmailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldLockUser_WhenDataIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new LockUserDto { Reason = "Violation", LockoutUntil = DateTimeOffset.UtcNow.AddDays(1) };
        var command = new LockUserCommand(userId, dto);
        var user = new ApplicationUser { Id = userId.ToString() };

        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(user, Role.SuperAdmin.ToString())).ReturnsAsync(false);
        _passwordServiceMock.Setup(x => x.LockUserAsync(user, dto.LockoutUntil.Value.UtcDateTime))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordServiceMock.Verify(x => x.LockUserAsync(user, dto.LockoutUntil.Value.UtcDateTime), Times.Once);
        _identityEmailServiceMock.Verify(x => x.SendAccountLockedEmailAsync(user, dto), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new LockUserDto { Reason = "Reason" };
        var command = new LockUserCommand(userId, dto);
        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId.ToString())).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowBadRequest_WhenLockingSuperAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new LockUserDto { Reason = "Reason" };
        var command = new LockUserCommand(userId, dto);
        var user = new ApplicationUser { Id = userId.ToString() };

        _userIdentityServiceMock.Setup(x => x.FindUserByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userIdentityServiceMock.Setup(x => x.IsInRoleAsync(user, Role.SuperAdmin.ToString())).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Cannot lock a SuperAdmin account.");
    }
}
