using Application.DTOs.Authentication;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IExternalAuthService
{
    Task<ApplicationUser> GoogleLoginAsync(GoogleLoginDto googleLoginDto);
}