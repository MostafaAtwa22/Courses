namespace API.RateLimiting.Options
{
    public sealed class SearchPolicyOptions
{
    public int PermitLimit        { get; init; } = 60;
    public int WindowMinutes      { get; init; } = 1;
    public int SegmentsPerWindow  { get; init; } = 4;
    public int QueueLimit         { get; init; } = 5;
}
}