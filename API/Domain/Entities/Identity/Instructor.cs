namespace Domain.Entities.Identity
{
    public class Instructor : BaseEntity
    {
        public string Bio { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string LinkedInProfileUrl { get; set; } = string.Empty;
        public string GitHubProfileUrl { get; set; } = string.Empty;
        public string CvUrl { get; set; } = string.Empty;
        public InstructorStatus Status { get; set; } = InstructorStatus.Pending;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<Course> Courses { get; set; } = [];
    }
}
