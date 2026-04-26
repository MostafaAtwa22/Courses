using Application.Common.Interfaces;

namespace Application.Features.Courses.Commands.Create
{
    public sealed class CreateCourseCommandHandler(ICourseRepository _repo) : IRequestHandler<CreateCourseCommand, Guid>
    {
        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            return await _repo.CreateAsync(request.Dto, cancellationToken);
        }
    }
}