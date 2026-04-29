namespace API.RateLimiting.Options
{
    public sealed class GlobalPolicyOptions
{
    public int PermitLimit        { get; init; } = 200;
    public int WindowMinutes      { get; init; } = 1;
    public int SegmentsPerWindow  { get; init; } = 4;
    public int QueueLimit         { get; init; } = 10;
}
}