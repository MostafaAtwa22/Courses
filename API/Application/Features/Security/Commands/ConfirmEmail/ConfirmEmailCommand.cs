using Application.DTOs.Security;

namespace Application.Features.Security.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(ConfirmEmailDto Dto) : IRequest<AuthResponseDto>;
}
