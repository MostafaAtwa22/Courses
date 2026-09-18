using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;

namespace Infrastructure.Identity;

public class InstructorProfileService(IInstructorRepository _instructorRepository) : IInstructorProfileService
{
    public async Task RemoveInstructorProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var hasCourses = await _instructorRepository.HasCoursesAsync(userId, cancellationToken);
        if (hasCourses)
        {
            throw new BadRequestException("Cannot remove instructor role: instructor has active courses");
        }

        await _instructorRepository.DeleteByUserIdAsync(userId, cancellationToken);
    }
}
