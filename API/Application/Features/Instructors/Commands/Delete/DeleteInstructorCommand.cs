using Domain.Enums.Identity;

namespace Application.Features.Instructors.Commands.Delete
{
    public sealed record DeleteInstructorCommand(Guid Id) 
        : IRequest, IRequireAuthorization
    {
        public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
        public bool RequireOwnership => false;
        public Guid ResourceId => Guid.Empty;
    }
}
