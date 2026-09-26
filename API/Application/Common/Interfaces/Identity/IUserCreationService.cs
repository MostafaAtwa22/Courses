using Application.DTOs.Authentication;
using Domain.Entities.Identity;
using Domain.Enums.Identity;

namespace Application.Common.Interfaces.Identity;

public interface IUserCreationService
{
    Task<ApplicationUser> CreateUserAsync(
        RegisterDto registerDto,
        UserCreationOptions options,
        CancellationToken cancellationToken = default);
}

public record UserCreationOptions
{
    public bool ConfirmEmail { get; init; } = true;
    public bool CreateStudentProfile { get; init; } = false;
    public bool AutoConfirmEmail { get; init; } = false;
}