using Application.DTOs.Authentication;

namespace Application.Features.Authentication.Commands.Register
{
    public sealed record CreateRegisterCommand(RegisterDto Dto) : IRequest;
}