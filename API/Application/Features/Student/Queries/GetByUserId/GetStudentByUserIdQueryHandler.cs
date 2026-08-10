using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetByUserId;

public sealed class GetStudentByUserIdQueryHandler(IStudentRepository _studentRepository)
    : IRequestHandler<GetStudentByUserIdQuery, StudentResponseDto?>
{
    public async Task<StudentResponseDto?> Handle(GetStudentByUserIdQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("Student", Guid.Parse(request.UserId));

        return await _studentRepository.GetByIdAsync(student.Id, cancellationToken);
    }
}
