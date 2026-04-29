namespace API.RateLimiting.Options
{
    public sealed class InstructorWritePolicyOptions
{
    public int TokenLimit            { get; init; } = 10;
    public int TokensPerPeriod       { get; init; } = 3;
    public int ReplenishmentHours    { get; init; } = 1;
    public int QueueLimit            { get; init; } = 0;
}
}