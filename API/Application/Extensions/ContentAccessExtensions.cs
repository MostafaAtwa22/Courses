using Application.DTOs.Content;

namespace Application.Extensions
{
    public static class ContentAccessExtensions
    {
        public static void RedactProtectedUrls(this IEnumerable<ContentResponseDto> contents, bool hasFullAccess)
        {
            if (hasFullAccess)
                return;

            foreach (var content in contents)
                if (!content.IsPreview)
                    content.ContentUrl = string.Empty;
        }

        public static void RedactProtectedUrl(this ContentResponseDto? content, bool hasFullAccess)
        {
            if (content == null || hasFullAccess)
                return;

            if (!content.IsPreview)
                content.ContentUrl = string.Empty;
        }
    }
}
