namespace Application.Features.Authorization.Commands.UpdateUserRoles;

public class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .Must(BeAValidGuid).WithMessage("UserId must be a valid GUID.");

        RuleFor(x => x.Dto.Roles)
            .NotEmpty()
            .WithMessage("At least one role must be provided.")
            .Must(HaveAtLeastOneSelectedRole).WithMessage("At least one role must be selected.");

        RuleForEach(x => x.Dto.Roles)
            .ChildRules(role =>
            {
                rule.RuleFor(r => r.RoleName)
                    .NotEmpty()
                    .WithMessage("Role name is required.");
            });
    }

    private bool BeAValidGuid(string userId)
    {
        return Guid.TryParse(userId, out _);
    }

    private bool HaveAtLeastOneSelectedRole(ICollection<CheckBoxRoleManageDto> roles)
    {
        return roles != null && roles.Any(r => r.IsSelected);
    }
}