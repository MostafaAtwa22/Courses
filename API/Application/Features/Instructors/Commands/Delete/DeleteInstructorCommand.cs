using Domain.Enums.Identity;

namespace Application.Features.Instructors.Commands.Delete
{
    public sealed record DeleteInstructorCommand(Guid Id)
        : IRequest, IRequireAuthorization
    {
        public string[] RequiredRoles => [];
        public string[] RequiredPermissions => [PermissionConstants.Build(Module.Instructor, CRUD.Delete)];
        public bool RequireOwnership => false;
        public Guid ResourceId => Guid.Empty;
    }
}
