
namespace Application.Features.Courses.Commands.Delete
{
    public sealed class DeleteCourseCommandHandler(ICourseRepository _repo) : IRequestHandler<DeleteCourseCommand>
    {
        public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _repo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Course), request.Id);

            await _repo.DeleteAsync(request.Id, cancellationToken);  
        }
    }
}