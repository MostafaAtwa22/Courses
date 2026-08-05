using Application.Common.Interfaces.Identity;
using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetByCourse
{
    public sealed record GetContentByCourseQuery(Guid CourseId, QueryParams QueryParams) 
        : IRequest<PaginatedResult<ContentResponseDto>>, IRequireEnrollment, IRequireContentAccess
    {
        public Guid ContentId => Guid.Empty;
        public bool AllowPreview => true;
    }
}
