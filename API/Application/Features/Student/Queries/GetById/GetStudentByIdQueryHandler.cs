using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetById;

public sealed class GetStudentByIdQueryHandler(IStudentRepository _studentRepository)
    : IRequestHandler<GetStudentByIdQuery, StudentResponseDto?>
{
    public async Task<StudentResponseDto?> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _studentRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}
