using Application.DTOs.Authorization.Permissions;
using Application.Features.Permissions.Commands;
using FluentValidation.TestHelper;

namespace Application.Tests.Permissions.Commands
{
    public class UpdateRolePermissionsCommandValidatorTests
    {
        private readonly UpdateRolePermissionsCommandValidator _validator;

        public UpdateRolePermissionsCommandValidatorTests()
        {
            _validator = new UpdateRolePermissionsCommandValidator();
        }

        [Fact]
        public void Should_NotHaveError_When_CommandIsValid()
        {
            var command = new UpdateRolePermissionsCommand(
                Guid.NewGuid().ToString(),
                [
                    new() { Name = "Permission:Role:Read", IsSelected = true },
                    new() { Name = "Permission:Role:Update", IsSelected = true }
                ]
            );
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_HaveError_When_IdIsEmpty()
        {
            var command = new UpdateRolePermissionsCommand(
                string.Empty,
                [new() { Name = "Permission:Role:Read", IsSelected = true }]
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_HaveError_When_IdIsInvalidGuid()
        {
            var command = new UpdateRolePermissionsCommand(
                "invalid-guid",
                [new() { Name = "Permission:Role:Read", IsSelected = true }]
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_HaveError_When_PermissionsDtoIsNull()
        {
            var command = new UpdateRolePermissionsCommand(
                Guid.NewGuid().ToString(),
                null!
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PermissionsDto);
        }

        [Fact]
        public void Should_HaveError_When_PermissionsDtoIsEmpty()
        {
            var command = new UpdateRolePermissionsCommand(
                Guid.NewGuid().ToString(),
                []
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PermissionsDto);
        }

        [Fact]
        public void Should_HaveError_When_NoPermissionIsSelected()
        {
            var command = new UpdateRolePermissionsCommand(
                Guid.NewGuid().ToString(),
                [
                    new() { Name = "Permission:Role:Read", IsSelected = false },
                    new() { Name = "Permission:Role:Update", IsSelected = false }
                ]
            );
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.PermissionsDto);
        }

    }
}
