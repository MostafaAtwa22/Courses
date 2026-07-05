using Application.Common.Interfaces;

namespace Infrastructure.BackgroundJobs
{
    public class DiscountJobService(ICourseDiscountRepository _discountRepository) : IDiscountJobService
    {
        public async Task DeactivateExpiredDiscountsAsync()
        {
            await _discountRepository.DeactivateExpiredAsync();
        }
    }
}
