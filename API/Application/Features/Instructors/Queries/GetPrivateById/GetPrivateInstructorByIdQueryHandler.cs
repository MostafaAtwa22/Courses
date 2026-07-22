using Application.Common.Exceptions;
using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;
using Domain.Enums.Identity;
using MediatR;

namespace Application.Features.Instructors.Queries.GetPrivateById
{
    public sealed class GetPrivateInstructorByIdQueryHandler(
        IInstructorRepository _repo,
        ICurrentUserService _currentUserService,
        IUserIdentityService _userIdentityService)
        : IRequestHandler<GetPrivateInstructorByIdQuery, InstructorPrivateResponseDto?>
    {
        public async Task<InstructorPrivateResponseDto?> Handle(
            GetPrivateInstructorByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedException("User is not authenticated.");

            var user = await _userIdentityService.FindUserByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedException("User not found.");

            bool isAdmin = await _userIdentityService.IsInRoleAsync(user, Role.Admin.ToString());
            bool isSuperAdmin = await _userIdentityService.IsInRoleAsync(user, Role.SuperAdmin.ToString());

            if (!isAdmin && !isSuperAdmin)
            {
                var instructor = await _repo.GetByUserIdAsync(userId, cancellationToken);
                if (instructor == null || instructor.Id != request.Id)
                {
                    throw new ForbiddenException("You are not authorized to view this instructor's private details.");
                }
            }

            return await _repo.GetPrivateByIdAsync(request.Id, cancellationToken);
        }
    }
}
