using Application.Common.Interfaces.Identity;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authentication.Commands.Register
{
    public sealed class CreateRegisterCommandHandler(
            UserManager<ApplicationUser> _userManager,
            IUserIdentityService _userIdentityService,
            IPasswordService _passwordService,
            IIdentityEmailService _identityEmailService) :
        IRequestHandler<CreateRegisterCommand>
    {
        public async Task Handle(CreateRegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _userIdentityService.IsEmailExistsAsync(request.Dto.Email))
                throw new BadRequestException("Email already exists.");

            if (await _userIdentityService.IsUserNameExistsAsync(request.Dto.UserName))
                throw new BadRequestException("Username already exists.");

            var user = request.Dto.ToApplicationUser();

            var result = await _userManager.CreateAsync(user, request.Dto.Password);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, request.Dto.Role.ToString());

            var token = await _passwordService.GenerateEmailConfirmationTokenAsync(user);
            await _identityEmailService.SendEmailConfirmationEmailAsync(user, token);
        }
    }
}