namespace API.RateLimiting.Options
{
    public sealed class AuthPolicyOptions
{
    public int PermitLimit    { get; init; } = 5;
    public int WindowMinutes  { get; init; } = 15;
    public int QueueLimit     { get; init; } = 0;
}
}