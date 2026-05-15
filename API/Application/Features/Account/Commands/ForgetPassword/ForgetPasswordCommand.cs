using Application.DTOs.Account;

namespace Application.Features.Account.Commands.ForgetPassword
{
    public record ForgetPasswordCommand(ForgetPasswordDto Dto) : IRequest;
}
