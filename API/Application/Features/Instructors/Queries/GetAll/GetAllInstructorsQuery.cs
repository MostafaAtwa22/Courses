using Application.DTOs.Instructor;
using Domain.Enums.Identity;
using Domain.Constants;

namespace Application.Features.Instructors.Queries.GetAll
{
    public sealed record GetAllInstructorsQuery(InstructorQueryParams Params)
        : IRequest<PaginatedResult<InstructorPrivateResponseDto>>, IRequireAuthorization
    {
        public string[] RequiredRoles => [];
        public string[] RequiredPermissions => [PermissionConstants.Build(Module.Instructor, CRUD.Read)];
        public bool RequireOwnership => false;
        public Guid ResourceId => Guid.Empty;
    }
}
