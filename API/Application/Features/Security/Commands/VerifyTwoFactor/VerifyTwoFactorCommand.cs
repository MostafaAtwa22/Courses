using Application.DTOs.Authentication;
using Application.DTOs.Security;

namespace Application.Features.Security.Commands.VerifyTwoFactor
{
    public record VerifyTwoFactorCommand(VerifyTwoFactorDto Dto) : IRequest<AuthResponseDto>;
}
