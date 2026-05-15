using Application.DTOs.Account;

namespace Application.Features.Account.Commands.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordDto Dto) : IRequest;
}
