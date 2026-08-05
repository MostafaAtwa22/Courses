using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Contents.Queries.GetBySection
{
    public sealed record GetContentBySectionQuery(Guid SectionId, Guid CourseId) 
        : IRequest<IReadOnlyList<ContentResponseDto>>, IRequireEnrollment, IRequireContentAccess
    {
        public Guid ContentId => Guid.Empty;
        public bool AllowPreview => true;
    }
}
