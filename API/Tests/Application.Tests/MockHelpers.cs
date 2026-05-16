using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests;

public static class MockHelpers
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
        return mgr;
    }
    public static Mock<SignInManager<TUser>> MockSignInManager<TUser>(Mock<UserManager<TUser>> userManager) where TUser : class
    {
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<TUser>>>();
        var schemes = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<TUser>>();

        return new Mock<SignInManager<TUser>>(
            userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object,
            confirmation.Object);
    }
}
