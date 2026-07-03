using Application.DTOs.Authentication;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
    Task<AuthResponseDto> GetAuthResponseAsync(ApplicationUser user);
    Task<AuthResponseDto> GenerateAuthWithRefreshTokenAsync(ApplicationUser user);
}
