namespace Application.Common.Interfaces.Identity;

public interface IInstructorProfileService
{
    Task RemoveInstructorProfileAsync(string userId, CancellationToken cancellationToken = default);
}
