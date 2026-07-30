namespace API.Swagger.Options
{
    public sealed class SwaggerOptions
    {
        public const string SectionName = "Swagger";

        public string Title { get; init; } = "EduFocus API";
        public string Version { get; init; } = "v1";
        public string Description { get; init; } = "EduFocus API documentation";
        public string DocumentName { get; init; } = "v1";
        public string SecuritySchemeName { get; init; } = "Bearer";
        public string BearerFormat { get; init; } = "JWT";
        public string Scheme { get; init; } = "bearer";
        public string HeaderName { get; init; } = "Authorization";
        public string SecurityDescription { get; init; } = "Enter a valid JWT bearer token.";
    }
}
