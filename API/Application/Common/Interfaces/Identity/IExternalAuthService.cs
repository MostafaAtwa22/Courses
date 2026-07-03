using Application.DTOs.Authentication;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IExternalAuthService
{
    Task<ApplicationUser> GoogleLoginAsync(GoogleLoginDto googleLoginDto);
    Task<ApplicationUser> FacebookLoginAsync(FacebookLoginDto facebookLoginDto);
    Task<ApplicationUser> GithubLoginAsync(GithubLoginDto githubLoginDto);
}