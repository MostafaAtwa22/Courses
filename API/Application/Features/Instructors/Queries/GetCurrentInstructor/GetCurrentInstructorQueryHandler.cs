using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetCurrentInstructor
{
    public sealed class GetCurrentInstructorQueryHandler(
        IInstructorRepository _repo,
        ICurrentUserService _currentUserService)
        : IRequestHandler<GetCurrentInstructorQuery, InstructorPrivateResponseDto?>
    {
        public async Task<InstructorPrivateResponseDto?> Handle(
            GetCurrentInstructorQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");
            
            return await _repo.GetPrivateByUserIdAsync(userId, cancellationToken);
        }
    }
}
