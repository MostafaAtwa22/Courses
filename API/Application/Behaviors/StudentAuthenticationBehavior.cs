using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;

namespace Application.Behaviors
{
    public class StudentAuthenticationBehavior<TRequest, TResponse>(
        ICurrentUserService _currentUserService,
        IStudentRepository _studentRepository)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequireStudent
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");

            var student = await _studentRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only students can perform this action.");

            if (request is IStudentInjectable studentRequest)
                studentRequest.StudentId = student.Id;

            return await next();
        }
    }
}
