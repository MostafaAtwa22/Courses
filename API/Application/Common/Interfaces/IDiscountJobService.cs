namespace Application.Common.Interfaces
{
    public interface IDiscountJobService
    {
        Task DeactivateExpiredDiscountsAsync();
    }
}
