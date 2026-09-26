using Application.Common.Interfaces.Identity;

namespace Application.Features.Admin.Commands.Create;

public sealed class CreateAdminCommandHandler(
        IUserCreationService _userCreationService
    ) : IRequestHandler<CreateAdminCommand>
{
    public async Task Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        var options = new UserCreationOptions
        {
            ConfirmEmail = false,
            CreateStudentProfile = false,
            AutoConfirmEmail = true
        };

        await _userCreationService.CreateUserAsync(request.Dto, options, cancellationToken);
    }
}