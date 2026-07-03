using Application.DTOs.Authentication;
using Domain.Entities.Identity;

namespace Application.Common.Interfaces.Identity;

public interface ILoginPipeline
{
    Task<AuthResponseDto> ExecuteAsync(ApplicationUser user);
}
