using Application.DTOs.Student;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IStudentRepository
{
    Task<Student?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<Student?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
    Task<StudentResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PaginatedResult<StudentResponseDto>> GetAllAsync(StudentQueryParams queryParams, CancellationToken ct = default);
    Task<Guid> CreateAsync(Student student, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Guid?> GetStudentIdByUserIdAsync(string userId, CancellationToken ct = default);
}
