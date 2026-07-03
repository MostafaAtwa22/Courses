using Application.Common.Models.Identity;
using Domain.Enums.Identity;

namespace Infrastructure.Identity.Authentication.Github;

public static class GithubUserInfoResultExtensions
{
    public static ExternalUser MapToGithub(this GithubValidator.GithubUserInfoResult githubUserInfo)
    {
        var nameParts = (githubUserInfo.Name ?? githubUserInfo.Login ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : "GithubUser";
        var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

        return new ExternalUser
        {
            Id = githubUserInfo.Id.ToString(),
            Email = githubUserInfo.Email ?? $"{githubUserInfo.Id}@github.local",
            FirstName = firstName,
            LastName = lastName,
            Provider = ExternalLoginProvider.Github
        };
    }
}
