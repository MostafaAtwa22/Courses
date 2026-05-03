using Application.Common.Models;
using Application.DTOs.Content;
using MediatR;

namespace Application.Features.Contents.Queries.GetByCourse
{
    public sealed record GetContentByCourseQuery(Guid CourseId, QueryParams QueryParams) : IRequest<PaginatedResult<ContentResponseDto>>;
}
