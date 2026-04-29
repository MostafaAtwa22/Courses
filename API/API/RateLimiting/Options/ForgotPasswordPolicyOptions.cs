namespace API.RateLimiting.Options
{
    public sealed class ForgotPasswordPolicyOptions
{
    public int PermitLimit   { get; init; } = 3;
    public int WindowHours   { get; init; } = 1;
    public int QueueLimit    { get; init; } = 0;
}
}