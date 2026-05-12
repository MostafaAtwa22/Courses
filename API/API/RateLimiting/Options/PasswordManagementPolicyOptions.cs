namespace API.RateLimiting.Options
{
    public sealed class PasswordManagementPolicyOptions
    {
        public int PermitLimit { get; set; } = 3;
        public int WindowMinutes { get; set; } = 15;
        public int QueueLimit { get; set; } = 0;
    }
}
