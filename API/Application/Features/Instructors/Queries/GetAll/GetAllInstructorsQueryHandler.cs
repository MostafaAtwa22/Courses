using Application.Common.Interfaces.Identity;
using Application.DTOs.Instructor;

namespace Application.Features.Instructors.Queries.GetAll
{
    public sealed class GetAllInstructorsQueryHandler(IInstructorRepository _repo)
        : IRequestHandler<GetAllInstructorsQuery, PaginatedResult<InstructorPrivateResponseDto>>
    {
        public async Task<PaginatedResult<InstructorPrivateResponseDto>> Handle(
            GetAllInstructorsQuery request, 
            CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync(request.Params, cancellationToken);
        }
    }
}
