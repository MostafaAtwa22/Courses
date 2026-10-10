using Domain.Enums.Identity;

namespace Application.Features.Instructors.Commands.ChangeStatus
{
    public sealed record ChangeInstructorStatusCommand(Guid Id, InstructorStatus Status)
        : IRequest, IRequireAuthorization
    {
        public string[] RequiredRoles => [];
        public string[] RequiredPermissions => [PermissionConstants.Build(Module.Instructor, CRUD.Update)];
        public bool RequireOwnership => false;
        public Guid ResourceId => Guid.Empty;
    }
}
