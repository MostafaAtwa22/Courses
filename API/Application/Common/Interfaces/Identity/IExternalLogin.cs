namespace Application.Common.Interfaces.Identity;
using static Google.Apis.Auth.GoogleJsonWebSignature;

public interface IExternalLogin
{
    Task<Payload?> ValidateGoogleAsync(string idToken);
}