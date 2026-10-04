using Application.DTOs.Instructor;
using Role = Domain.Enums.Identity.Role;

namespace Application.Features.Instructors.Queries.GetPrivateById
{
    public sealed record GetPrivateInstructorByIdQuery(Guid Id) : IRequest<InstructorPrivateResponseDto?>, IRequireAuthorization
    {
        public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
        public bool RequireOwnership => true;
        public Guid ResourceId => Id;
    }
}
