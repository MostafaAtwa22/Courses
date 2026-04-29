namespace API.RateLimiting.Options
{
    public sealed class MediaPolicyOptions
{
    public int PermitLimit  { get; init; } = 3;
    public int QueueLimit   { get; init; } = 2;
}
}