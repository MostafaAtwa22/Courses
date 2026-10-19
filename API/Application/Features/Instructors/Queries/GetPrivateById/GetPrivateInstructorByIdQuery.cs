using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPrivateById
{
    public sealed record GetPrivateInstructorByIdQuery(Guid Id) : IRequest<InstructorPrivateResponseDto?>, IRequireAuthorization
    {
        public string[] RequiredRoles => [];
        public string[] RequiredPermissions => ["Permission:Instructor:Read"];
        public bool RequireOwnership => true;
        public Guid ResourceId => Id;
    }
}
