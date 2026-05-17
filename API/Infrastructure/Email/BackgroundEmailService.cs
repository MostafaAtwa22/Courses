using Application.Common.Interfaces.Email;
using Application.DTOs.Email;
using Hangfire;

namespace Infrastructure.Email
{
    public class BackgroundEmailService : IEmailService
    {
        public Task SendEmailAsync(EmailMessageDto dto)
        {
            BackgroundJob.Enqueue<EmailService>(
                "emails",
                service => service.SendEmailAsync(dto));

            return Task.CompletedTask; 
        }
    }
}
