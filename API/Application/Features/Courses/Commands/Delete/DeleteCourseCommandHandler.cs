
namespace Application.Features.Courses.Commands.Delete
{
    public sealed class DeleteCourseCommandHandler(
        ICourseRepository _repo, 
        IFileService _fileService) : IRequestHandler<DeleteCourseCommand>
    {
        public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _repo.GetEntityByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Course), request.Id);

            await _fileService.DeleteAsync(course.PictureUrl);

            await _repo.DeleteAsync(request.Id, cancellationToken);
        }
    }
}