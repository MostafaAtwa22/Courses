using Application.Common.Interfaces;
using Application.Common.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public sealed class UrlProvider(IOptions<UrlsOptions> urlsOptions) : IUrlProvider
    {
        public string? GetFullUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return relativePath;
            }

            return $"{urlsOptions.Value.API}/{relativePath}";
        }
    }
}
