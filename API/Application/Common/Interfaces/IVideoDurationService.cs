using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces
{
    public interface IVideoDurationService
    {
        Task<double> GetDurationAsync(IFormFile videoFile, CancellationToken cancellationToken);
    }
}
