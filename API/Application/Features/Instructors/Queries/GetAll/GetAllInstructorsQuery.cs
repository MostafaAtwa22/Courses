using Application.Common.Interfaces.Identity;
using Application.Common.Models;
using Application.DTOs.Instructor;
using Domain.Enums.Identity;

namespace Application.Features.Instructors.Queries.GetAll
{
    public sealed record GetAllInstructorsQuery(QueryParams Params) 
        : IRequest<PaginatedResult<InstructorPrivateResponseDto>>, IRequireAuthorization
    {
        public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
        public bool RequireOwnership => false;
        public Guid ResourceId => Guid.Empty;
    }
}
