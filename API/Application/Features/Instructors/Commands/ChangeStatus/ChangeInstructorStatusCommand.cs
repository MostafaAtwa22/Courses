using Application.Common.Interfaces.Identity;
using Domain.Enums;

namespace Application.Features.Instructors.Commands.ChangeStatus
{
    public sealed record ChangeInstructorStatusCommand(Guid Id, InstructorStatus Status) 
        : IRequest, IRequireAuthorization
    {
        public string[] RequiredRoles => [Role.Admin.ToString(), Role.SuperAdmin.ToString()];
        public bool RequireOwnership => false;
        public Guid ResourceId => Guid.Empty;
    }
}
