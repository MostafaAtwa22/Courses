namespace Application.Common.Options
{
    public sealed class UrlsOptions
    {
        public const string SectionName = "Urls";
        public string API { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
    }
}