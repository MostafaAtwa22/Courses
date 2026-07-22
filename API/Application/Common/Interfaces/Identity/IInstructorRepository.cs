using Application.DTOs.Instructor;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface IInstructorRepository
    {
        Task<Instructor?> GetByUserIdAsync(string userId, CancellationToken ct = default);
        Task<Instructor?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        Task<InstructorPublicResponseDto?> GetPublicByIdAsync(Guid id, CancellationToken ct = default);
        Task<InstructorPrivateResponseDto?> GetPrivateByIdAsync(Guid id, CancellationToken ct = default);
        Task<Guid> CreateAsync(Instructor instructor, CancellationToken ct = default);
        Task UpdateAsync(Instructor instructor, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
