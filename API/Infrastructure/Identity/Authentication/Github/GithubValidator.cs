using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Identity;
using Domain.Enums.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity.Authentication.Github;

public class GithubValidator : IExternalLoginValidator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GithubOptions _options;

    public GithubValidator(IHttpClientFactory httpClientFactory, IOptions<GithubOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public ExternalLoginProvider Provider => ExternalLoginProvider.Github;

    public async Task<ExternalUser> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        var parts = token.Split('|', 2);
        if (parts.Length != 2)
            throw new UnauthorizedException("Invalid Github login payload.");

        var code = parts[0];
        var redirectUri = parts[1];

        var accessToken = await ExchangeCodeForTokenAsync(code, redirectUri, cancellationToken);

        return await GetUserInfoAsync(accessToken, cancellationToken);
    }

    private async Task<string> ExchangeCodeForTokenAsync(string code, string redirectUri, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Identity-Server");

        var tokenRequest = new
        {
            client_id = _options.ClientId,
            client_secret = _options.ClientSecret,
            code,
            redirect_uri = redirectUri
        };

        var response = await httpClient.PostAsJsonAsync(
            "https://github.com/login/oauth/access_token",
            tokenRequest,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedException("Failed to exchange Github authorization code for access token.");

        var tokenResponse = await response.Content.ReadFromJsonAsync<GithubTokenResponse>(cancellationToken: cancellationToken);

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            throw new UnauthorizedException("Github did not return a valid access token.");

        if (!string.IsNullOrEmpty(tokenResponse.Error))
            throw new UnauthorizedException($"Github token exchange error: {tokenResponse.ErrorDescription}");

        return tokenResponse.AccessToken;
    }

    private async Task<ExternalUser> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Identity-Server");
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");

        var userInfo = await httpClient.GetFromJsonAsync<GithubUserInfoResult>(
            "https://api.github.com/user", cancellationToken);

        if (userInfo == null || userInfo.Id == 0)
            throw new UnauthorizedException("Failed to retrieve Github user information.");

        if (string.IsNullOrEmpty(userInfo.Email))
        {
            var emails = await httpClient.GetFromJsonAsync<List<GithubUserEmailResult>>(
                "https://api.github.com/user/emails", cancellationToken);

            var primaryEmail = emails?.FirstOrDefault(e => e.Primary && e.Verified)?.Email
                            ?? emails?.FirstOrDefault(e => e.Primary)?.Email;

            if (!string.IsNullOrEmpty(primaryEmail))
                userInfo.Email = primaryEmail;
        }

        if (string.IsNullOrEmpty(userInfo.Email))
            throw new UnauthorizedException("Could not retrieve a verified email from your Github account. Please ensure your email is public or verified on Github.");

        return userInfo.MapToGithub();
    }

    private class GithubTokenResponse
    {
        [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
        [JsonPropertyName("token_type")]   public string? TokenType   { get; set; }
        [JsonPropertyName("scope")]        public string? Scope        { get; set; }
        [JsonPropertyName("error")]        public string? Error        { get; set; }
        [JsonPropertyName("error_description")] public string? ErrorDescription { get; set; }
    }

    public class GithubUserInfoResult
    {
        [JsonPropertyName("id")]    public long    Id    { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("name")]  public string? Name  { get; set; }
        [JsonPropertyName("login")] public string? Login { get; set; }
    }

    public class GithubUserEmailResult
    {
        [JsonPropertyName("email")]    public string Email    { get; set; } = string.Empty;
        [JsonPropertyName("primary")]  public bool   Primary  { get; set; }
        [JsonPropertyName("verified")] public bool   Verified { get; set; }
    }
}
