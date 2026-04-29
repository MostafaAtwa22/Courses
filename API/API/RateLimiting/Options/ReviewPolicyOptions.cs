namespace API.RateLimiting.Options
{
    public sealed class ReviewPolicyOptions
{
    public int PermitLimit   { get; init; } = 10;
    public int WindowHours   { get; init; } = 24;
    public int QueueLimit    { get; init; } = 0;
}
}