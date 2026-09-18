using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.Common.Mappings;

namespace Infrastructure.Identity;

public class StudentProfileService(IStudentRepository _studentRepository) : IStudentProfileService
{
    public async Task EnsureStudentProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var existingStudent = await _studentRepository.GetByUserIdAsync(userId, cancellationToken);
        if (existingStudent is null)
        {
            var student = userId.ToStudent();
            await _studentRepository.CreateAsync(student, cancellationToken);
        }
    }

    public async Task RemoveStudentProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var hasEnrollments = await _studentRepository.HasEnrollmentsAsync(userId, cancellationToken);
        if (hasEnrollments)
            throw new BadRequestException("Cannot remove student role: user has active enrollments");

        await _studentRepository.DeleteByUserIdAsync(userId, cancellationToken);
    }
}
