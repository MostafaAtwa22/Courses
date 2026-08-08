namespace Application.Features.Student.Commands.DeleteStudent;

public sealed class DeleteStudentCommandHandler(IStudentRepository _studentRepository)
    : IRequestHandler<DeleteStudentCommand>
{
    public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
    {
        await _studentRepository.DeleteAsync(request.Id, cancellationToken);
    }
}
