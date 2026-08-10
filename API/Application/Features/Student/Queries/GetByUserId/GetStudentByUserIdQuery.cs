using Application.DTOs.Student;

namespace Application.Features.Student.Queries.GetByUserId;

public sealed record GetStudentByUserIdQuery(string UserId) : IRequest<StudentResponseDto?>;
