using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetById;

public sealed record GetStudentByIdQuery(Guid Id) : IRequest<StudentResponseDto?>;
