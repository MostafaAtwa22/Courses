using Application.Common.Interfaces.Identity;

namespace Application.Features.Authentication.Commands.Register
{
    public sealed class CreateRegisterCommandHandler(
            IUserCreationService _userCreationService) :
        IRequestHandler<CreateRegisterCommand>
    {
        public async Task Handle(CreateRegisterCommand request, CancellationToken cancellationToken)
        {
            var options = new UserCreationOptions
            {
                ConfirmEmail = true,
                CreateStudentProfile = true,
                AutoConfirmEmail = false
            };

            await _userCreationService.CreateUserAsync(request.Dto, options, cancellationToken);
        }
    }
}