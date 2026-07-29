using Application.Common.Interfaces.Identity;

namespace Application.Features.Instructors.Commands.ChangeStatus
{
    public sealed class ChangeInstructorStatusCommandHandler(IInstructorRepository _repo)
        : IRequestHandler<ChangeInstructorStatusCommand>
    {
        public async Task Handle(ChangeInstructorStatusCommand request, CancellationToken cancellationToken)
        {
            await _repo.UpdateStatusAsync(request.Id, request.Status, cancellationToken);
        }
    }
}
