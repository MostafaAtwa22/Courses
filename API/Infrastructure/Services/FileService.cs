using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services
{
    public class FileService(IWebHostEnvironment _env) : IFileService
    {
        public async Task<string> UploadAsync(Stream fileStream, string fileName, string folder)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            
            var sanitizedFileName = Path.GetFileName(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await fileStream.CopyToAsync(stream);
            
            return $"{folder}/{uniqueFileName}";
        }

        public async Task DeleteAsync(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, filePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            await Task.CompletedTask;
        }
    }
}