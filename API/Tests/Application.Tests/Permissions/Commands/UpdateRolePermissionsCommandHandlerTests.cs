using Application.Common.Exceptions;
using Application.Common.Interfaces.Cache;
using Application.Common.Interfaces;
using Application.DTOs.Authorization.Permissions;
using Application.Features.Permissions.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Permissions.Commands
{
    public class UpdateRolePermissionsCommandHandlerTests
    {
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IPermissionRepository> _permissionRepositoryMock;
        private readonly Mock<IAppCache> _cacheMock;
        private readonly UpdateRolePermissionsCommandHandler _handler;

        public UpdateRolePermissionsCommandHandlerTests()
        {
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                null,
                null,
                null,
                null
            );
            _permissionRepositoryMock = new Mock<IPermissionRepository>();
            _cacheMock = new Mock<IAppCache>();
            _handler = new UpdateRolePermissionsCommandHandler(
                _roleManagerMock.Object,
                _permissionRepositoryMock.Object,
                _cacheMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldUpdatePermissions_WhenDataIsValid()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();
            var role = new IdentityRole { Id = roleId, Name = "Admin" };
            var permissions = new List<PermissionCheckboxDto>
            {
                new() { Name = "Permission:Role:Read", IsSelected = true },
                new() { Name = "Permission:Role:Update", IsSelected = true }
            };
            var command = new UpdateRolePermissionsCommand(roleId, permissions);

            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(role);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _permissionRepositoryMock.Verify(
                x => x.UpdateRolePermissionsAsync(
                    role,
                    It.Is<IEnumerable<string>>(p => p.Count() == 2),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldInvalidateCache_WhenUpdateSucceeds()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();
            var role = new IdentityRole { Id = roleId, Name = "Admin" };
            var permissions = new List<PermissionCheckboxDto>
            {
                new() { Name = "Permission:Role:Read", IsSelected = true }
            };
            var command = new UpdateRolePermissionsCommand(roleId, permissions);

            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(role);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _cacheMock.Verify(x => x.RemoveByTagAsync("permissions", It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(x => x.RemoveAsync($"permissions:role:{roleId}", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();
            var permissions = new List<PermissionCheckboxDto>
            {
                new() { Name = "Permission:Role:Read", IsSelected = true }
            };
            var command = new UpdateRolePermissionsCommand(roleId, permissions);

            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync((IdentityRole?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _permissionRepositoryMock.Verify(
                x => x.UpdateRolePermissionsAsync(It.IsAny<IdentityRole>(), It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldOnlyPassSelectedPermissions()
        {
            // Arrange
            var roleId = Guid.NewGuid().ToString();
            var role = new IdentityRole { Id = roleId, Name = "Admin" };
            var permissions = new List<PermissionCheckboxDto>
            {
                new() { Name = "Permission:Role:Read", IsSelected = true },
                new() { Name = "Permission:Role:Update", IsSelected = false },
                new() { Name = "Permission:Role:Delete", IsSelected = true }
            };
            var command = new UpdateRolePermissionsCommand(roleId, permissions);

            _roleManagerMock.Setup(x => x.FindByIdAsync(roleId)).ReturnsAsync(role);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _permissionRepositoryMock.Verify(
                x => x.UpdateRolePermissionsAsync(
                    role,
                    It.Is<IEnumerable<string>>(p => p.Count() == 2 && p.Contains("Permission:Role:Read") && p.Contains("Permission:Role:Delete")),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
