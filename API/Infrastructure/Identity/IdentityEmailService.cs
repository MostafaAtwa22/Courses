using System.Text;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Application.DTOs.Account;
using Application.DTOs.Email;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity
{
    public class IdentityEmailService(
        IEmailService _emailService,
        IOptions<UrlsOptions> _urlsOptions) : IIdentityEmailService
    {
        public async Task Send2FAEmailAsync(ApplicationUser user, string code)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "TwoFactorAuthentication.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var htmlBody = template
                .Replace("{{UserName}}", user.UserName)
                .Replace("{{Otp}}", code);

            var emailRequest = new EmailMessageDto
            {
                To = user.Email!,
                Subject = "Your authentication code",
                Body = htmlBody
            };

            await _emailService.SendEmailAsync(emailRequest);
        }

        public async Task SendAccountLockedEmailAsync(ApplicationUser user, LockUserDto dto)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "AccountLocked.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var lockoutUntilStr = dto.LockoutUntil.HasValue 
                ? dto.LockoutUntil.Value.ToString("f") 
                : "Indefinitely";

            var htmlBody = template
                .Replace("{{UserName}}", user.UserName)
                .Replace("{{Reason}}", dto.Reason)
                .Replace("{{LockoutUntil}}", lockoutUntilStr);

            var emailRequest = new EmailMessageDto
            {
                To = user.Email!,
                Subject = "Account Locked",
                Body = htmlBody
            };

            await _emailService.SendEmailAsync(emailRequest);
        }

        public async Task SendAccountUnlockedEmailAsync(ApplicationUser user)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "AccountUnlocked.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var htmlBody = template
                .Replace("{{UserName}}", user.UserName);

            var emailRequest = new EmailMessageDto
            {
                To = user.Email!,
                Subject = "Account Unlocked",
                Body = htmlBody
            };

            await _emailService.SendEmailAsync(emailRequest);
        }

        public async Task SendPasswordResetEmailAsync(ApplicationUser user, string token)
        {
            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token)
            );

            var baseUrl = _urlsOptions.Value.Client;
            var resetLink = $"{baseUrl}/auth/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={Uri.EscapeDataString(encodedToken)}";
            
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "ForgetPassword.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var htmlBody = template
                .Replace("{{UserName}}", user.UserName)
                .Replace("{{ResetLink}}", resetLink);

            var emailRequest = new EmailMessageDto
            {
                To = user.Email!,
                Subject = "Reset Your Password",
                Body = htmlBody
            };

            await _emailService.SendEmailAsync(emailRequest);
        }

        public async Task SendEmailConfirmationEmailAsync(ApplicationUser user, string token)
        {
            var baseUrl = _urlsOptions.Value.Client;
            var confirmationPageUrl = $"{baseUrl}/auth/confirm-email?email={Uri.EscapeDataString(user.Email!)}";

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "EmailConfirmation.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var htmlBody = template
                .Replace("{{UserName}}", user.UserName)
                .Replace("{{ConfirmationCode}}", token)
                .Replace("{{ConfirmationPageUrl}}", confirmationPageUrl);

            var emailRequest = new EmailMessageDto
            {
                To = user.Email!,
                Subject = "Confirm your email",
                Body = htmlBody
            };

            await _emailService.SendEmailAsync(emailRequest);
        }
    }
}
