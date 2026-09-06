using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public class VideoDurationService : IVideoDurationService
    {
        public async Task<double> GetDurationAsync(IFormFile videoFile, CancellationToken cancellationToken)
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), videoFile.FileName);
            using (var fileStream = System.IO.File.Create(tempFilePath))
            {
                await videoFile.CopyToAsync(fileStream, cancellationToken);
            }

            var tagFile = TagLib.File.Create(tempFilePath);
            var duration = tagFile.Properties.Duration.TotalSeconds;

            System.IO.File.Delete(tempFilePath);

            return duration;
        }
    }
}
