using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetPrivateById
{
    public sealed class GetPrivateInstructorByIdQueryHandler(
        IInstructorRepository _repo)
        : IRequestHandler<GetPrivateInstructorByIdQuery, InstructorPrivateResponseDto?>
    {
        public async Task<InstructorPrivateResponseDto?> Handle(
            GetPrivateInstructorByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetPrivateByIdAsync(request.Id, cancellationToken);
        }
    }
}
