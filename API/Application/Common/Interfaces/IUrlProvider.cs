namespace Application.Common.Interfaces
{
    public interface IUrlProvider
    {
        string? GetFullUrl(string? relativePath);
    }
}
