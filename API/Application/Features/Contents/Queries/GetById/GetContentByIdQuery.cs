using Application.DTOs.Content;

namespace Application.Features.Contents.Queries.GetById
{
    public sealed record GetContentByIdQuery(Guid Id, Guid CourseId) 
        : IRequest<ContentResponseDto?>, IRequireEnrollment
    {
        public Guid ContentId => Id;
        public bool AllowPreview => true;
    }
}
