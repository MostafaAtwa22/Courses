namespace Application.Common.Interfaces.Identity;


public interface IAuthService : ITokenService, IRefreshTokenRepository, IUserIdentityService, IPasswordService
{
}
