using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IRefreshTokenRepository
{
    RefreshToken GenerateRefreshToken(string jwtId);
    Task AddAsync(ApplicationUser user, RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenAsync(string token);
}
