using Application.DTOs.Authorization.Permissions;
using Domain.Constants;

namespace Application.Features.Permissions.Commands
{
    public class UpdateRolePermissionsCommandValidator : AbstractValidator<UpdateRolePermissionsCommand>
    {
        public UpdateRolePermissionsCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Role ID is required.")
                .Must(BeAValidGuid).WithMessage("Role ID must be a valid GUID.");

            RuleFor(x => x.PermissionsDto)
                .NotEmpty()
                .WithMessage("At least one permission must be provided.")
                .Must(HaveAtLeastOneSelectedPermission).WithMessage("At least one permission must be selected.");

            RuleForEach(x => x.PermissionsDto)
                .ChildRules(permission =>
                {
                    permission.RuleFor(p => p.Name)
                        .NotEmpty()
                        .WithMessage("Permission name is required.")
                        .Must(BeAValidPermissionFormat).WithMessage("Permission must be in format: Permission:Module:Action")
                        .Must(BeAValidPermissionName).WithMessage("Permission must be a valid permission from the system.");
                });
        }

        private bool BeAValidGuid(string id)
        {
            return Guid.TryParse(id, out _);
        }

        private bool HaveAtLeastOneSelectedPermission(ICollection<PermissionCheckboxDto> permissions)
        {
            return permissions != null && permissions.Any(p => p.IsSelected);
        }

        private bool BeAValidPermissionFormat(string permissionName)
        {
            if (string.IsNullOrEmpty(permissionName))
                return false;

            var parts = permissionName.Split(':');
            return parts.Length == 3 && parts[0] == "Permission";
        }

        private bool BeAValidPermissionName(string permissionName)
        {
            if (string.IsNullOrEmpty(permissionName))
                return false;

            var allPermissions = PermissionConstants.GenerateAllPermissions();
            return allPermissions.Contains(permissionName);
        }
    }

}