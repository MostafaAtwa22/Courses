using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetAll;

public sealed record GetStudentsQuery(StudentQueryParams Params) : IRequest<PaginatedResult<StudentResponseDto>>;
