using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetAll;

public sealed class GetStudentsQueryHandler(IStudentRepository _studentRepository)
    : IRequestHandler<GetStudentsQuery, PaginatedResult<StudentResponseDto>>
{
    public async Task<PaginatedResult<StudentResponseDto>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
    {
        return await _studentRepository.GetAllAsync(request.Params, cancellationToken);
    }
}
