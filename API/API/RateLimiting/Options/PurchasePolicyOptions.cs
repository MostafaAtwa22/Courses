namespace API.RateLimiting.Options
{
    public sealed class PurchasePolicyOptions
{
    public int TokenLimit            { get; init; } = 5;
    public int TokensPerPeriod       { get; init; } = 2;
    public int ReplenishmentMinutes  { get; init; } = 10;
    public int QueueLimit            { get; init; } = 0;
}
}