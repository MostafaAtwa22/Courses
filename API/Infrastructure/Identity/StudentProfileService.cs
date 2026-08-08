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
}
