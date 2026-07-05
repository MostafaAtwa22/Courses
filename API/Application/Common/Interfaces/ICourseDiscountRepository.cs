using Application.DTOs.Course;
using Domain.Entities;

namespace Application.Common.Interfaces
{
    public interface ICourseDiscountRepository
    {
        Task<IEnumerable<CourseDiscountDto>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);
        Task<CourseDiscount?> GetEntityByIdAsync(Guid id, CancellationToken ct = default);
        Task<Guid> AddAsync(CourseDiscount discount, CancellationToken ct = default);
        Task UpdateAsync(CourseDiscount discount, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task DeactivateExpiredAsync(CancellationToken ct = default);
    }
}
