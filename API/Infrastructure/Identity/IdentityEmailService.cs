using System;
using System.IO;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Identity;
using Application.Common.Options;
using Application.DTOs.Email;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Infrastructure.Identity
{
    public class IdentityEmailService(
        UserManager<ApplicationUser> _userManager,
        IEmailService _emailService,
        IOptions<UrlsOptions> _urlsOptions) : IIdentityEmailService
    {
        public async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var baseUrl = _urlsOptions.Value.Client;
            var encodedToken = Uri.EscapeDataString(emailConfirmationToken);
            var confirmationLink = $"{baseUrl}/authentication/confirm-email?userId={user.Id}&token={encodedToken}";

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Email", "Templates", "EmailConfirmation.html");
            var template = await File.ReadAllTextAsync(templatePath);
            
            var htmlBody = template
                .Replace("{{UserName}}", user.UserName)
                .Replace("{{ConfirmationLink}}", confirmationLink);

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
