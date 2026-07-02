using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Interfaces.Identity;
using Application.Common.Models.Identity;
using Domain.Enums.Identity;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity.Authentication.Google;

public class GoogleValidator : IExternalLoginValidator
{
    private readonly GoogleOptions _options;

    public GoogleValidator(IOptions<GoogleOptions> options)
    {
        _options = options.Value;
    }

    public ExternalLoginProvider Provider => ExternalLoginProvider.Google;

    public async Task<ExternalUser> ValidateAsync(string token, CancellationToken cancellationToken = default)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_options.ClientId]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings)
            ?? throw new UnauthorizedException("Invalid Google token payload.");

        return payload.MapToGoogle();
    }
}
