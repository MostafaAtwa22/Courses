using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Identity;
using Domain.Enums.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity.Authentication.Facebook;

public class FacebookValidator : IExternalLoginValidator
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly FacebookOptions _options;

    public FacebookValidator(IHttpClientFactory httpClientFactory, IOptions<FacebookOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public ExternalLoginProvider Provider => ExternalLoginProvider.Facebook;

    public async Task<ExternalUser> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient("FacebookAPI");
        httpClient.BaseAddress = new Uri("https://graph.facebook.com/");

        var appAccessToken = $"{_options.AppId}|{_options.AppSecret}";
        var debugTokenUrl = $"debug_token?input_token={token}&access_token={appAccessToken}";

        var debugResponse = await httpClient.GetFromJsonAsync<FacebookTokenValidationResult>(debugTokenUrl, cancellationToken);

        if (debugResponse?.Data == null || !debugResponse.Data.IsValid)
            throw new UnauthorizedException("Invalid Facebook access token.");

        var userInfoUrl = $"me?fields=id,email,first_name,last_name,picture&access_token={token}";
        var userInfo = await httpClient.GetFromJsonAsync<FacebookUserInfoResult>(userInfoUrl, cancellationToken);

        if (userInfo == null || string.IsNullOrEmpty(userInfo.Id))
            throw new UnauthorizedException("Failed to retrieve Facebook user information.");

        return userInfo.MapToFacebook();
    }

    private class FacebookTokenValidationResult
    {
        [JsonPropertyName("data")] public FacebookTokenValidationData Data { get; set; } = new();
    }

    private class FacebookTokenValidationData
    {
        [JsonPropertyName("is_valid")] public bool IsValid { get; set; }
    }

    public class FacebookUserInfoResult
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("first_name")] public string FirstName { get; set; } = string.Empty;
        [JsonPropertyName("last_name")] public string LastName { get; set; } = string.Empty;
        [JsonPropertyName("picture")] public FacebookPicture Picture { get; set; } = new();
    }

    public class FacebookPicture
    {
        [JsonPropertyName("data")] public FacebookPictureData Data { get; set; } = new();
    }

    public class FacebookPictureData
    {
        [JsonPropertyName("url")] public string Url { get; set; } = string.Empty;
    }
}
