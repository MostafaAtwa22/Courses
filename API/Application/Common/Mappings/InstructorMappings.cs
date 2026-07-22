using Application.DTOs.Instructor;
using Domain.Entities.Identity;

namespace Application.Common.Mappings
{
    public static class InstructorMappings
    {
        public static Instructor ToEntity(this InstructorCreateDto dto, string cvUrl, string userId)
        {
            return new Instructor
            {
                Bio = dto.Bio,
                Title = dto.Title,
                LinkedInProfileUrl = dto.LinkedInProfileUrl,
                GitHubProfileUrl = dto.GitHubProfileUrl,
                CvUrl = cvUrl,
                UserId = userId
            };
        }

        public static void UpdateEntity(this InstructorUpdateDto dto, Instructor instructor, string? cvUrl = null)
        {
            instructor.Bio = dto.Bio;
            instructor.Title = dto.Title;
            instructor.LinkedInProfileUrl = dto.LinkedInProfileUrl;
            instructor.GitHubProfileUrl = dto.GitHubProfileUrl;

            if (cvUrl is not null)
                instructor.CvUrl = cvUrl;
        }
    }
}
