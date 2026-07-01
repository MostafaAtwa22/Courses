using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Microsoft.Extensions.Options;
using Google.Apis.Auth;

namespace Infrastructure.Identity;

public class ExternalLogin : IExternalLogin
{   
    private GoogleOptions  _googleOptions;

    public ExternalLogin(IOptions<GoogleOptions> googleOptions)
    {
        _googleOptions = googleOptions.Value;
    }
    
    public async Task<Payload?> ValidateGoogleAsync(string idToken)
    {
        var settings = new ValidationSettings()
        {
            Audience = [_googleOptions.ClientId]
        };
        var payload = await ValidateAsync(idToken, settings);
        
        return payload;
    }
}