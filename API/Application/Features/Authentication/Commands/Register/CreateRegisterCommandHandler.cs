using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Application.Common.Interfaces.Identity;

namespace Application.Features.Authentication.Commands.Register
{
    public sealed class CreateRegisterCommandHandler(
            UserManager<ApplicationUser> _userManager,
            IAuthService _authService,
            IIdentityEmailService _identityEmailService) :
        IRequestHandler<CreateRegisterCommand>
    {
        public async Task Handle(CreateRegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _authService.IsEmailExistsAsync(request.Dto.Email))
                throw new BadRequestException("Email already exists.");

            if (await _authService.IsUserNameExistsAsync(request.Dto.UserName))
                throw new BadRequestException("Username already exists.");

            var user = request.Dto.ToApplicationUser();

            var result = await _userManager.CreateAsync(user, request.Dto.Password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, request.Dto.Role.ToString());

            await _identityEmailService.SendConfirmationEmailAsync(user);
        }
    }
}