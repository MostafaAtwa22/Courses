using Application.DTOs.Email;

namespace Application.Common.Interfaces.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDto Dto);
    }
}