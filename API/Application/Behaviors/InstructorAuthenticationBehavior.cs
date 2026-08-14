using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Identity;
using MediatR;

namespace Application.Behaviors
{
    public class InstructorAuthenticationBehavior<TRequest, TResponse>(
        ICurrentUserService _currentUserService,
        IInstructorRepository _instructorRepository)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequireInstructor
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId 
                ?? throw new UnauthorizedException("You must be logged in.");

            var instructor = await _instructorRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new ForbiddenException("Only instructors can perform this action.");

            if (request is IInstructorInjectable instructorRequest)
                instructorRequest.InstructorId = instructor.Id;

            return await next();
        }
    }
}
