namespace Application.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string folder);
        Task DeleteAsync(string filePath);
    }
}