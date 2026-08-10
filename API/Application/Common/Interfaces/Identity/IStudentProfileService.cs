namespace Application.Common.Interfaces.Identity;

public interface IStudentProfileService
{
    Task EnsureStudentProfileAsync(string userId, CancellationToken cancellationToken = default);
}
