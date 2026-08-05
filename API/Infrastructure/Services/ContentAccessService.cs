using Application.Common.Interfaces.Identity;
using Domain.Enums.Identity;

namespace Infrastructure.Services
{
    public class ContentAccessService(
        ICurrentUserService _currentUserService,
        IUserIdentityService _userIdentityService,
        IEnrollmentRepository _enrollmentRepository,
        IInstructorRepository _instructorRepository)
        : IContentAccessService
    {
        public async Task<bool> HasFullCourseContentAccessAsync(Guid courseId, CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = await _userIdentityService.FindUserByIdAsync(userId);
            if (user == null)
                return false;

            if (await _userIdentityService.IsInRoleAsync(user, Role.Admin.ToString()) ||
                await _userIdentityService.IsInRoleAsync(user, Role.SuperAdmin.ToString()))
            {
                return true;
            }

            var instructorId = await _enrollmentRepository.GetInstructorIdByCourseIdAsync(courseId, cancellationToken);
            if (instructorId.HasValue)
            {
                var instructor = await _instructorRepository.GetByUserIdAsync(userId, cancellationToken);
                if (instructor != null && instructor.Id == instructorId.Value)
                    return true;
            }

            var isEnrolled = await _enrollmentRepository.IsEnrolledByUserIdAsync(userId, courseId, cancellationToken);
            return isEnrolled;
        }
    }
}
