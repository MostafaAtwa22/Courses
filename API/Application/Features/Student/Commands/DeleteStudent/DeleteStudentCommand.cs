namespace Application.Features.Student.Commands.DeleteStudent;

public sealed record DeleteStudentCommand(Guid Id) : IRequest;
