namespace Application.Features.Courses.Commands.Update
{
    public sealed class UpdateCourseCommandHandler(ICourseRepository _repo) : IRequestHandler<UpdateCourseCommand>
    {
        public async Task Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _repo.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Course), request.Id);
                
            await _repo.UpdateAsync(request.Id, request.Dto, cancellationToken);
        }
    }
}