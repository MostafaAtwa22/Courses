namespace Application.DTOs.Account
{
    public class LockUserDto
    {
        public DateTimeOffset? LockoutUntil { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}