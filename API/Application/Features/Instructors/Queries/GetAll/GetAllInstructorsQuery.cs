using Application.DTOs.Instructor;
using Role = Domain.Enums.Identity.Role;

namespace Application.Features.Instructors.Queries.GetAll
{
    public sealed record GetAllInstructorsQuery(InstructorQueryParams Params) 
        : IRequest<PaginatedResult<InstructorPrivateResponseDto>>, IRequireAuthorization
    {
        public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
        public bool RequireOwnership => false;
        public Guid ResourceId => Guid.Empty;
    }
}
