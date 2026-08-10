using Application.DTOs.Progress;

namespace Application.Common.Mappings
{
    public static class ProgressMappings
    {
        public static CourseProgressDto ToDto(this CourseProgressSummaryDto summary, HashSet<Guid> completedContentIds)
        {
            return new CourseProgressDto
            {
                CourseId = summary.CourseId,
                CompletedCount = summary.CompletedCount,
                TotalCount = summary.TotalCount,
                PercentComplete = summary.PercentComplete,
                CompletedContentIds = completedContentIds.ToList()
            };
        }
    }
}
