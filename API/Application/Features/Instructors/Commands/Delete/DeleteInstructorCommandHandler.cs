using Application.Common.Interfaces.Identity;

namespace Application.Features.Instructors.Commands.Delete
{
    public sealed class DeleteInstructorCommandHandler(IInstructorRepository _repo)
        : IRequestHandler<DeleteInstructorCommand>
    {
        public async Task Handle(DeleteInstructorCommand request, CancellationToken cancellationToken)
        {
            await _repo.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
