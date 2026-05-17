using Application.Common.Interfaces.Email;
using Hangfire;
using Application.Common.Options;
using Application.DTOs.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Email
{
    public class EmailService(IOptions<EmailOptions> _options) : IEmailService
    {
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 120, 300 })]
        [Queue("emails")]
        public async Task SendEmailAsync(EmailMessageDto Dto)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_options.Value.SenderName, _options.Value.SenderEmail));
            emailMessage.To.Add(MailboxAddress.Parse(Dto.To));
            emailMessage.Subject = Dto.Subject;

            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = Dto.Body
            };

            using var stmpClient = new SmtpClient();

            await stmpClient.ConnectAsync(
                _options.Value.Host,
                _options.Value.Port,
                SecureSocketOptions.StartTls
            );

            await stmpClient.AuthenticateAsync(_options.Value.SenderEmail, _options.Value.Password);
            await stmpClient.SendAsync(emailMessage);
            await stmpClient.DisconnectAsync(true);
        }
    }
}