using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Authentication;
using Application.Features.Authentication.Commands.Login;
using Domain.Entities.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Constant = Domain.Constants.IdentityConstants;

namespace Application.Tests.Authentication.Commands;

public class LoginHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    private readonly CreateLoginCommandHandler _handler;

    public LoginHandlerTests()
    {
        _userManagerMock = MockHelpers.MockUserManager<ApplicationUser>();
        _signInManagerMock = MockHelpers.MockSignInManager(_userManagerMock);
        _authServiceMock = new Mock<IAuthService>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        
        _handler = new CreateLoginCommandHandler(
            _userManagerMock.Object, 
            _signInManagerMock.Object,
            _authServiceMock.Object, 
            _twoFactorServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenLoginIsSuccessful()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
        var command = new CreateLoginCommand(dto);
        var user = new ApplicationUser { Email = dto.Email };
        var authResponse = new AuthResponseDto { Email = user.Email, Token = "token" };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, dto.Password, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        _authServiceMock.Setup(x => x.GetAuthResponseAsync(user)).ReturnsAsync(authResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(authResponse);
    }

    [Fact]
    public async Task Handle_ShouldReturn2FAResponse_When2FAIsRequired()
    {
        // Arrange
        var dto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
        var command = new CreateLoginCommand(dto);
        var user = new ApplicationUser { Email = dto.Email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, dto.Password, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.RequiresTwoFactor.Should().BeTrue();
        result.Provider.Should().Be(Constant.EmailOtpProvider);
        _twoFactorServiceMock.Verify(x => x.SendOtpAsync(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorized_WhenUserNotFound()
    {
        // Arrange
        var dto = new LoginDto { Email = "wrong@example.com", Password = "Password123!" };
        var command = new CreateLoginCommand(dto);
        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((ApplicationUser?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>().WithMessage("Invalid email or password.");
    }

    [Fact]
    public async Task Handle_ShouldThrowEmailNotConfirmed_WhenEmailIsNotConfirmed()
    {
        // Arrange
        var dto = new LoginDto { Email = "unconfirmed@example.com", Password = "Password123!" };
        var command = new CreateLoginCommand(dto);
        var user = new ApplicationUser { Email = dto.Email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EmailNotConfirmedException>().WithMessage("Email confirmation is required.");
    }

    [Fact]
    public async Task Handle_ShouldThrowAccountLocked_WhenUserIsLockedOut()
    {
        // Arrange
        var dto = new LoginDto { Email = "locked@example.com", Password = "Password123!" };
        var command = new CreateLoginCommand(dto);
        var user = new ApplicationUser { Email = dto.Email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, dto.Password, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AccountLockedException>().WithMessage("Account is locked. Please try again later.");
    }
}
