using Application.Common.Models.Identity;
using Domain.Enums.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IExternalLoginValidator
{
    ExternalLoginProvider Provider { get; }

    Task<ExternalUser> ValidateAsync(string token, CancellationToken cancellationToken = default);
}
