using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity
{
    public interface IInstructorRepository
    {
        Task<Instructor?> GetByUserIdAsync(string userId, CancellationToken ct = default);
        Task<Instructor?> GetByIdAsync(Guid id, CancellationToken ct = default);
    }
}
