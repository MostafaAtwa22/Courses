namespace Application.Common.Options
{
    public class EmailOptions
    {
        public const string SectionName = "EmailSettings";
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
    }
}