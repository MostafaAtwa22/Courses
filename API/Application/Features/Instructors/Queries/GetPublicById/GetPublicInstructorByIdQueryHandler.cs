using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPublicById
{
    public sealed class GetPublicInstructorByIdQueryHandler(IInstructorRepository _repo)
        : IRequestHandler<GetPublicInstructorByIdQuery, InstructorPublicResponseDto?>
    {
        public async Task<InstructorPublicResponseDto?> Handle(
            GetPublicInstructorByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetPublicByIdAsync(request.Id, cancellationToken);
        }
    }
}
